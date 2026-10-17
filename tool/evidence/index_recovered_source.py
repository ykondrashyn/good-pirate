#!/usr/bin/env python3
"""Freeze the GUID-preserving 1.13 recovered source imported into Unity."""

from __future__ import annotations

import csv
import hashlib
import re
from pathlib import Path


REPOSITORY_ROOT = Path(__file__).resolve().parents[2]
OUTPUT = REPOSITORY_ROOT / "docs/evidence/recovered-source-manifest.tsv"
BASELINE_ASSETS = Path(
    "../good_pirate_1_13_evidence/asset-ripper/"
    "working-import-baseline/ExportedProject/Assets"
)
SOURCE_ROOTS = (
    REPOSITORY_ROOT / "Assets/Editor/AssetRipperPatches",
    REPOSITORY_ROOT / "Assets/Plugins/Assembly-CSharp-firstpass",
    REPOSITORY_ROOT / "Assets/Scripts",
)
GUID_PATTERN = re.compile(r"^guid:\s*([0-9a-f]{32})\s*$", re.MULTILINE)


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for chunk in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def script_guid(path: Path) -> str:
    metadata = Path(str(path) + ".meta")
    if not metadata.is_file():
        raise RuntimeError(f"Recovered source metadata is missing: {metadata}")
    match = GUID_PATTERN.search(metadata.read_text(encoding="utf-8"))
    if match is None:
        raise RuntimeError(f"Recovered source metadata has no GUID: {metadata}")
    return match.group(1)


def main() -> int:
    rows: list[list[str]] = []
    seen_guids: dict[str, str] = {}
    for root in SOURCE_ROOTS:
        for source in sorted(root.rglob("*.cs"), key=lambda item: item.as_posix()):
            relative = source.relative_to(REPOSITORY_ROOT).as_posix()
            asset_relative = source.relative_to(REPOSITORY_ROOT / "Assets")
            baseline_source = BASELINE_ASSETS / asset_relative
            if not baseline_source.is_file():
                raise RuntimeError(
                    f"Working-import baseline source is missing: {baseline_source}"
                )
            guid = script_guid(source)
            previous = seen_guids.get(guid)
            if previous is not None:
                raise RuntimeError(
                    f"Duplicate recovered script GUID {guid}: {previous}, {relative}"
                )
            seen_guids[guid] = relative
            rows.append(
                [
                    relative,
                    guid,
                    sha256(baseline_source),
                    "Good Pirate 1.13 APK",
                    "AssetRipper 1.3.14 Hybrid Level 2 working-import baseline",
                ]
            )

    OUTPUT.parent.mkdir(parents=True, exist_ok=True)
    with OUTPUT.open("w", encoding="utf-8", newline="") as stream:
        writer = csv.writer(stream, delimiter="\t", lineterminator="\n")
        writer.writerow(["path", "script_guid", "sha256", "authority", "provenance"])
        writer.writerows(rows)
    print(f"RECOVERED_SOURCE_INDEX_OK files={len(rows)} output={OUTPUT}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
