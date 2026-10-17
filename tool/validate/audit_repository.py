#!/usr/bin/env python3
"""Audit repository isolation, payload boundaries, and donor independence."""

from __future__ import annotations

import os
import csv
import sys
from collections import defaultdict
from pathlib import Path

from common import GENERATED_TOP_LEVEL_DIRS, REPO_ROOT, Results, check_mode, git_output, hash_file, is_relative_to, load_config, repository_files


FORBIDDEN_PAYLOAD_SUFFIXES = {
    ".aab",
    ".apk",
    ".apks",
    ".dex",
    ".so",
    ".unitypackage",
    ".xapk",
}
FORBIDDEN_PAYLOAD_NAMES = {
    "dump.cs",
    "global-metadata.dat",
    "libil2cpp.so",
    "script.json",
    "stringsliteral.json",
}
DONOR_COMPARE_SUFFIXES = {
    ".anim",
    ".asset",
    ".controller",
    ".cs",
    ".fontsettings",
    ".jpeg",
    ".jpg",
    ".jslib",
    ".json",
    ".mat",
    ".mp3",
    ".ogg",
    ".otf",
    ".physicsmaterial2d",
    ".png",
    ".prefab",
    ".shader",
    ".spriteatlas",
    ".ttf",
    ".unity",
    ".wav",
}
STRONG_116_MARKERS = (
    "1.16.2",
    "DokdoBrowser",
    "GoodPirateM2",
    "GoodPirateSaveV1",
    "M2VerticalSlice",
)
TEXT_SUFFIXES = {
    ".asmdef",
    ".asset",
    ".cs",
    ".json",
    ".jslib",
    ".md",
    ".prefab",
    ".shader",
    ".txt",
    ".unity",
    ".yaml",
    ".yml",
}


def relative(path: Path) -> str:
    return path.relative_to(REPO_ROOT).as_posix()


def summarize_paths(paths: list[str], limit: int = 12) -> str:
    unique_paths = sorted(set(paths))
    examples = unique_paths[:limit]
    detail = f"count={len(unique_paths)}"
    if examples:
        detail += " | " + " | ".join(examples)
    remaining = len(unique_paths) - len(examples)
    if remaining:
        detail += f" | and {remaining} more"
    return detail


def main() -> int:
    mode = check_mode(sys.argv)
    config = load_config()["repository"]
    results = Results("repository boundary")

    git_root = git_output("rev-parse", "--show-toplevel")
    if git_root.returncode == 0 and Path(git_root.stdout.strip()).resolve() == REPO_ROOT.resolve():
        results.pass_("REPO_GIT_ROOT", f"independent Git root at {REPO_ROOT}")
    else:
        detail = git_root.stderr.strip() or git_root.stdout.strip() or "not a Git worktree"
        results.fail("REPO_GIT_ROOT", detail)

    git_entry = REPO_ROOT / ".git"
    if git_entry.is_dir() and not git_entry.is_symlink():
        results.pass_("REPO_GIT_DIRECTORY", ".git is a local directory, not a linked worktree file")
    else:
        results.fail("REPO_GIT_DIRECTORY", ".git must be an independent directory")

    allowed = set(config["allowed_top_level"])
    observed = {path.name for path in REPO_ROOT.iterdir() if path.name != ".git"}
    ignored_generated: list[str] = []
    unexpected: list[str] = []
    for name in sorted(observed - allowed):
        ignored = git_output("check-ignore", "-q", "--", name)
        if name in GENERATED_TOP_LEVEL_DIRS and ignored.returncode == 0:
            ignored_generated.append(name)
        else:
            unexpected.append(name)
    if unexpected:
        results.fail("REPO_TOP_LEVEL", f"unexpected paths: {', '.join(unexpected)}")
    else:
        detail = f"all source paths are allowed and {len(ignored_generated)} generated Unity directories are ignored"
        results.pass_("REPO_TOP_LEVEL", detail)

    hidden_source_roots: list[str] = []
    for source_root in ("docs/evidence", "tool/evidence"):
        if (REPO_ROOT / source_root).exists():
            ignored = git_output("check-ignore", "-q", "--", source_root)
            if ignored.returncode == 0:
                hidden_source_roots.append(source_root)
    if hidden_source_roots:
        results.fail(
            "REPO_SOURCE_VISIBILITY",
            f"source or index roots hidden by .gitignore: {', '.join(hidden_source_roots)}",
        )
    else:
        results.pass_("REPO_SOURCE_VISIBILITY", "evidence tools and indexes remain eligible for version control")

    escaping_links: list[str] = []
    for current_root, dir_names, file_names in os.walk(REPO_ROOT, followlinks=False):
        current = Path(current_root)
        if current == REPO_ROOT:
            dir_names[:] = [
                name
                for name in dir_names
                if name != ".git" and name not in GENERATED_TOP_LEVEL_DIRS
            ]
        for name in [*dir_names, *file_names]:
            path = current / name
            if not path.is_symlink():
                continue
            try:
                target = path.resolve(strict=True)
            except FileNotFoundError:
                escaping_links.append(f"{relative(path)} -> missing target")
                continue
            if not is_relative_to(target, REPO_ROOT.resolve()):
                escaping_links.append(f"{relative(path)} -> {target}")
    if escaping_links:
        results.fail("REPO_SYMLINK_BOUNDARY", " | ".join(escaping_links))
    else:
        results.pass_("REPO_SYMLINK_BOUNDARY", "no repository symlink escapes the Git root")

    embedded_payloads: list[str] = []
    forbidden_directory_payloads: list[str] = []
    for path in repository_files():
        rel = path.relative_to(REPO_ROOT)
        rel_lower_parts = tuple(part.lower() for part in rel.parts)
        if path.suffix.lower() in FORBIDDEN_PAYLOAD_SUFFIXES or path.name.lower() in FORBIDDEN_PAYLOAD_NAMES:
            embedded_payloads.append(rel.as_posix())
        if any(part in {"analysis", "decompiled", "extracted", "original-export"} for part in rel_lower_parts):
            forbidden_directory_payloads.append(rel.as_posix())
        if "evidence" in rel_lower_parts:
            allowed_docs_index = (
                len(rel.parts) >= 3
                and rel.parts[:2] == ("docs", "evidence")
                and path.suffix.lower() in {".json", ".md", ".tsv"}
            )
            allowed_tool_source = (
                len(rel.parts) >= 3
                and rel.parts[:2] == ("tool", "evidence")
                and (
                    path.suffix.lower() in {".cs", ".md", ".py", ".sh"}
                )
            )
            if not allowed_docs_index and not allowed_tool_source:
                forbidden_directory_payloads.append(rel.as_posix())
    payload_findings = sorted(set(embedded_payloads + forbidden_directory_payloads))
    if payload_findings:
        results.fail("REPO_NO_EMBEDDED_PAYLOAD", summarize_paths(payload_findings))
    else:
        results.pass_("REPO_NO_EMBEDDED_PAYLOAD", "no APK, extracted, or raw evidence payload is embedded")

    authorized_merges: dict[str, str] = {}
    merge_ledger = REPO_ROOT / "docs" / "source-merge-ledger.tsv"
    if merge_ledger.exists():
        with merge_ledger.open("r", encoding="utf-8", newline="") as stream:
            for row in csv.DictReader(stream, delimiter="\t"):
                destination = row.get("destination_path", "")
                after_hash = row.get("source_sha256_after", "")
                if destination and after_hash:
                    authorized_merges[destination] = after_hash
    declared_recovered: dict[str, str] = {}
    recovered_manifest = REPO_ROOT / "docs/evidence/recovered-source-manifest.tsv"
    if recovered_manifest.exists():
        with recovered_manifest.open("r", encoding="utf-8", newline="") as stream:
            for row in csv.DictReader(stream, delimiter="\t"):
                destination = row.get("path", "")
                source_hash = row.get("sha256", "")
                if destination and source_hash:
                    declared_recovered[destination] = source_hash
    decompiled_roots = (
        "Assets/Editor/AssetRipperPatches/",
        "Assets/Plugins/Assembly-CSharp-firstpass/",
        "Assets/Scripts/",
    )
    undeclared_decompiled: list[str] = []
    for path in repository_files(REPO_ROOT / "Assets"):
        rel = relative(path)
        if path.suffix.lower() != ".cs" or not rel.startswith(decompiled_roots):
            continue
        expected_hash = authorized_merges.get(rel, declared_recovered.get(rel))
        if expected_hash != hash_file(path):
            undeclared_decompiled.append(rel)
    if undeclared_decompiled:
        results.fail("REPO_NO_DECOMPILED_PAYLOAD", summarize_paths(undeclared_decompiled))
    else:
        results.pass_(
            "REPO_NO_DECOMPILED_PAYLOAD",
            "all recovered source is frozen by manifest or an explicit merge row",
        )

    marker_hits: list[str] = []
    scan_roots = [REPO_ROOT / name for name in ("Assets", "Packages", "ProjectSettings")]
    for scan_root in scan_roots:
        if not scan_root.exists():
            continue
        for path in repository_files(scan_root):
            if path.suffix.lower() not in TEXT_SUFFIXES or path.stat().st_size > 4 * 1024 * 1024:
                continue
            try:
                text = path.read_text(encoding="utf-8")
            except UnicodeDecodeError:
                continue
            for marker in STRONG_116_MARKERS:
                if marker in text:
                    marker_hits.append(f"{relative(path)} contains {marker}")
    if marker_hits:
        results.fail("REPO_NO_116_MARKERS", " | ".join(marker_hits))
    else:
        results.pass_("REPO_NO_116_MARKERS", "no strong 1.16 runtime or configuration markers found")

    donor_root = Path(config["forbidden_donor_root"])
    authority_assets = Path(config["authoritative_asset_root"])
    authorized_donor_files = config.get("authorized_donor_files", {})
    current_assets = REPO_ROOT / "Assets"
    if not donor_root.exists():
        results.warn("REPO_DONOR_FINGERPRINT", f"donor root unavailable: {donor_root}")
    elif donor_root.resolve() == REPO_ROOT.resolve():
        results.fail("REPO_DONOR_FINGERPRINT", "configured donor root resolves to the current repository")
    elif not current_assets.exists():
        results.skip("REPO_DONOR_FINGERPRINT", "Assets directory is not present yet")
    elif not authority_assets.exists():
        results.fail("REPO_DONOR_FINGERPRINT", f"1.13 authority assets unavailable: {authority_assets}")
    else:
        donor_by_size: dict[int, list[Path]] = defaultdict(list)
        donor_assets = donor_root / "Assets"
        for donor_file in repository_files(donor_assets):
            if donor_file.suffix.lower() in DONOR_COMPARE_SUFFIXES:
                donor_by_size[donor_file.stat().st_size].append(donor_file)

        donor_hash_cache: dict[Path, str] = {}
        uncorroborated_matches: list[str] = []
        corroborated_matches = 0
        authorized_matches = 0
        compared = 0
        for current_file in repository_files(current_assets):
            if current_file.suffix.lower() not in DONOR_COMPARE_SUFFIXES:
                continue
            candidates = donor_by_size.get(current_file.stat().st_size, [])
            if not candidates:
                continue
            compared += 1
            current_hash = hash_file(current_file)
            matching_donor: Path | None = None
            for donor_file in candidates:
                donor_hash = donor_hash_cache.get(donor_file)
                if donor_hash is None:
                    donor_hash = hash_file(donor_file)
                    donor_hash_cache[donor_file] = donor_hash
                if current_hash == donor_hash:
                    matching_donor = donor_file
                    break
            if matching_donor is None:
                continue
            asset_rel = current_file.relative_to(current_assets)
            authority_file = authority_assets / asset_rel
            if authority_file.is_file() and hash_file(authority_file) == current_hash:
                corroborated_matches += 1
            elif (
                isinstance(authorized_donor_files.get(relative(current_file)), dict)
                and authorized_donor_files[relative(current_file)].get("sha256") == current_hash
            ):
                authorized_matches += 1
            else:
                donor_rel = matching_donor.relative_to(donor_root).as_posix()
                uncorroborated_matches.append(f"{relative(current_file)} == {donor_rel}")
        if uncorroborated_matches:
            examples = uncorroborated_matches[:12]
            remaining = len(uncorroborated_matches) - len(examples)
            detail = " | ".join(examples)
            if remaining:
                detail += f" | and {remaining} more"
            results.fail("REPO_DONOR_FINGERPRINT", detail)
        else:
            results.pass_(
                "REPO_DONOR_FINGERPRINT",
                f"no uncorroborated 1.16 match, {corroborated_matches} shared files "
                f"match exact 1.13 authority, {authorized_matches} user-authorized exception",
            )

    if mode == "release":
        head = git_output("rev-parse", "--verify", "HEAD")
        if head.returncode == 0:
            results.pass_("RELEASE_COMMIT", f"HEAD {head.stdout.strip()}")
        else:
            results.fail("RELEASE_COMMIT", "repository has no committed HEAD")

        status = git_output("status", "--porcelain=v1", "--untracked-files=all")
        if status.returncode != 0:
            results.fail("RELEASE_CLEAN_TREE", status.stderr.strip() or "git status failed")
        elif status.stdout:
            paths = [line[3:] for line in status.stdout.splitlines()]
            results.fail("RELEASE_CLEAN_TREE", summarize_paths(paths))
        else:
            results.pass_("RELEASE_CLEAN_TREE", "worktree is clean")

    return results.finish()


if __name__ == "__main__":
    raise SystemExit(main())
