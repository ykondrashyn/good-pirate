#!/usr/bin/env python3
"""Plan or apply conservative 1.13 source-file replacements.

AssetRipper owns Unity script paths and their .meta GUIDs. ILSpy owns readable
method bodies. This tool joins them only when both trees expose the same exact
set of top-level C# types in a file. Ambiguous files are reported for AST-level
or manual review rather than guessed.
"""

from __future__ import annotations

import argparse
import csv
import hashlib
import json
import re
import shutil
import sys
from dataclasses import asdict, dataclass
from pathlib import Path


TYPE_RE = re.compile(
    r"\b(?:public|internal|protected|private|static|abstract|sealed|partial|new|unsafe|readonly\s+)*"
    r"\b(class|struct|interface|enum)\s+([A-Za-z_][A-Za-z0-9_]*)\b"
)
NAMESPACE_RE = re.compile(r"\bnamespace\s+([A-Za-z_][A-Za-z0-9_.]*)\s*\{")


@dataclass(frozen=True)
class SourceFile:
    path: Path
    sha256: str
    top_level_types: tuple[str, ...]


@dataclass(frozen=True)
class MergeRow:
    type_set: str
    donor_path: str
    destination_path: str
    donor_sha256: str
    destination_sha256: str
    meta_path: str
    action: str
    reason: str


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def mask_non_code(source: str) -> str:
    """Blank comments and literals while preserving positions and newlines."""
    chars = list(source)
    index = 0
    state = "code"
    while index < len(chars):
        char = chars[index]
        following = chars[index + 1] if index + 1 < len(chars) else ""
        if state == "code":
            if char == "/" and following == "/":
                chars[index] = chars[index + 1] = " "
                index += 2
                state = "line_comment"
                continue
            if char == "/" and following == "*":
                chars[index] = chars[index + 1] = " "
                index += 2
                state = "block_comment"
                continue
            if char == '"':
                chars[index] = " "
                index += 1
                state = "string"
                continue
            if char == "'":
                chars[index] = " "
                index += 1
                state = "char"
                continue
        elif state == "line_comment":
            if char == "\n":
                state = "code"
            else:
                chars[index] = " "
            index += 1
            continue
        elif state == "block_comment":
            if char == "*" and following == "/":
                chars[index] = chars[index + 1] = " "
                index += 2
                state = "code"
                continue
            if char != "\n":
                chars[index] = " "
            index += 1
            continue
        elif state in {"string", "char"}:
            if char == "\\" and following:
                chars[index] = chars[index + 1] = " "
                index += 2
                continue
            terminator = '"' if state == "string" else "'"
            if char == terminator:
                chars[index] = " "
                state = "code"
            elif char != "\n":
                chars[index] = " "
            index += 1
            continue
        index += 1
    return "".join(chars)


def brace_depths(source: str) -> list[int]:
    depths = [0] * (len(source) + 1)
    depth = 0
    for index, char in enumerate(source):
        depths[index] = depth
        if char == "{":
            depth += 1
        elif char == "}":
            depth = max(0, depth - 1)
    depths[len(source)] = depth
    return depths


def matching_brace(source: str, opening: int) -> int:
    depth = 0
    for index in range(opening, len(source)):
        if source[index] == "{":
            depth += 1
        elif source[index] == "}":
            depth -= 1
            if depth == 0:
                return index
    return len(source)


def top_level_types(path: Path) -> tuple[str, ...]:
    source = mask_non_code(path.read_text(encoding="utf-8-sig", errors="strict"))
    depths = brace_depths(source)
    namespaces: list[tuple[int, int, int, str]] = []
    for match in NAMESPACE_RE.finditer(source):
        opening = source.find("{", match.start(), match.end())
        namespaces.append((opening, matching_brace(source, opening), depths[opening] + 1, match.group(1)))

    result: list[str] = []
    for match in TYPE_RE.finditer(source):
        type_depth = depths[match.start()]
        containing = [item for item in namespaces if item[0] < match.start() < item[1]]
        containing.sort(key=lambda item: item[0])
        namespace = containing[-1] if containing else None
        expected_depth = namespace[2] if namespace else 0
        if type_depth != expected_depth:
            continue
        name = match.group(2)
        result.append(f"{namespace[3]}.{name}" if namespace else name)
    return tuple(sorted(set(result)))


def scan(root: Path) -> list[SourceFile]:
    files: list[SourceFile] = []
    for path in sorted(root.rglob("*.cs"), key=lambda item: item.as_posix()):
        types = top_level_types(path)
        if types:
            files.append(SourceFile(path.resolve(), sha256(path), types))
    return files


def index(files: list[SourceFile]) -> dict[tuple[str, ...], list[SourceFile]]:
    result: dict[tuple[str, ...], list[SourceFile]] = {}
    for item in files:
        result.setdefault(item.top_level_types, []).append(item)
    return result


def make_plan(donor_root: Path, destination_root: Path) -> list[MergeRow]:
    donors = index(scan(donor_root))
    destinations = index(scan(destination_root))
    rows: list[MergeRow] = []
    for type_set in sorted(set(donors) | set(destinations)):
        donor_items = donors.get(type_set, [])
        destination_items = destinations.get(type_set, [])
        type_label = ",".join(type_set)
        if len(donor_items) != 1 or len(destination_items) != 1:
            reason = f"donor_count={len(donor_items)},destination_count={len(destination_items)}"
            for donor in donor_items or [None]:
                for destination in destination_items or [None]:
                    rows.append(
                        MergeRow(
                            type_label,
                            str(donor.path) if donor else "",
                            str(destination.path) if destination else "",
                            donor.sha256 if donor else "",
                            destination.sha256 if destination else "",
                            str(destination.path) + ".meta" if destination else "",
                            "review",
                            reason,
                        )
                    )
            continue
        donor = donor_items[0]
        destination = destination_items[0]
        meta = Path(str(destination.path) + ".meta")
        if not meta.is_file():
            action, reason = "review", "destination_meta_missing"
        elif donor.sha256 == destination.sha256:
            action, reason = "unchanged", "source_hashes_match"
        else:
            action, reason = "replace", "exact_top_level_type_set"
        rows.append(
            MergeRow(
                type_label,
                str(donor.path),
                str(destination.path),
                donor.sha256,
                destination.sha256,
                str(meta),
                action,
                reason,
            )
        )
    return rows


def write_tsv(path: Path, rows: list[MergeRow]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as stream:
        writer = csv.DictWriter(stream, fieldnames=list(asdict(rows[0]).keys()) if rows else [
            "type_set", "donor_path", "destination_path", "donor_sha256",
            "destination_sha256", "meta_path", "action", "reason"
        ], delimiter="\t", lineterminator="\n")
        writer.writeheader()
        for row in rows:
            writer.writerow(asdict(row))


def apply_plan(rows: list[MergeRow]) -> int:
    replacements = 0
    for row in rows:
        if row.action != "replace":
            continue
        destination = Path(row.destination_path)
        if sha256(destination) != row.destination_sha256:
            raise RuntimeError(f"destination changed after planning: {destination}")
        if not Path(row.meta_path).is_file():
            raise RuntimeError(f"destination metadata disappeared: {row.meta_path}")
        shutil.copyfile(row.donor_path, destination)
        replacements += 1
    return replacements


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--donor", type=Path, required=True)
    parser.add_argument("--destination", type=Path, required=True)
    parser.add_argument("--report", type=Path, required=True)
    parser.add_argument("--apply", action="store_true")
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    if not args.donor.is_dir() or not args.destination.is_dir():
        print("donor and destination must be directories", file=sys.stderr)
        return 2
    rows = make_plan(args.donor, args.destination)
    write_tsv(args.report, rows)
    replacements = apply_plan(rows) if args.apply else 0
    summary = {
        "files": len(rows),
        "replace": sum(row.action == "replace" for row in rows),
        "unchanged": sum(row.action == "unchanged" for row in rows),
        "review": sum(row.action == "review" for row in rows),
        "applied": replacements,
    }
    print(json.dumps(summary, sort_keys=True))
    return 1 if summary["review"] else 0


if __name__ == "__main__":
    raise SystemExit(main())

