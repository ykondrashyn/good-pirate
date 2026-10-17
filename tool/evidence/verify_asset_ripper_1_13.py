#!/usr/bin/env python3
"""Verify two AssetRipper exports and publish their deterministic census."""

from __future__ import annotations

import csv
import hashlib
from pathlib import Path
import shutil
import sys


EXTERNAL_ROOT = Path("../good_pirate_1_13_evidence")


def sha256_file(file_path: Path) -> str:
    digest = hashlib.sha256()
    with file_path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def rows(file_path: Path) -> list[dict[str, str]]:
    with file_path.open(encoding="utf-8", newline="") as source:
        return list(csv.DictReader(source, delimiter="\t"))


def main() -> int:
    repository_root = Path(__file__).resolve().parents[2]
    reports_root = EXTERNAL_ROOT / "asset-ripper-reports"
    manifest_a = reports_root / "run-a/asset-ripper-export-manifest.tsv"
    manifest_b = reports_root / "run-b/asset-ripper-export-manifest.tsv"
    census_a = reports_root / "run-a/asset-ripper-census.tsv"
    census_b = reports_root / "run-b/asset-ripper-census.tsv"
    left = {row["relative_path"]: row for row in rows(manifest_a)}
    right = {row["relative_path"]: row for row in rows(manifest_b)}
    differences = [
        key
        for key in sorted(set(left) | set(right))
        if key not in left
        or key not in right
        or left[key]["normalized_bytes"] != right[key]["normalized_bytes"]
        or left[key]["normalized_sha256"] != right[key]["normalized_sha256"]
    ]
    if differences:
        raise RuntimeError("AssetRipper exports differ: " + ", ".join(differences[:20]))
    if census_a.read_bytes() != census_b.read_bytes():
        raise RuntimeError("AssetRipper census differs between runs")

    evidence = repository_root / "docs/evidence"
    evidence.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(manifest_a, evidence / "asset-ripper-export-manifest.tsv")
    shutil.copyfile(census_a, evidence / "asset-ripper-census.tsv")

    archive_census = EXTERNAL_ROOT / "runs/run-a/reports/content-census.tsv"
    archive_lines = archive_census.read_text(encoding="utf-8").splitlines()
    asset_lines = census_a.read_text(encoding="utf-8").splitlines()
    combined = "\n".join(archive_lines + asset_lines[1:]) + "\n"
    (repository_root / "docs/content-census.tsv").write_text(
        combined, encoding="utf-8", newline="\n"
    )

    manifest_rows = rows(manifest_a)
    census_rows = rows(census_a)
    scene_count = sum(
        1 for row in census_rows if row["record_type"] == "asset_ripper_item" and row["category"] == "unity_scene"
    )
    map_count = sum(
        1 for row in census_rows if row["record_type"] == "asset_ripper_item" and row["category"] == "resources_map"
    )
    report = f"""# AssetRipper 1.13 determinism

Two independent AssetRipper 1.3.14 processes exported the two clean APK
extractions to identical normalized file views. Raw exports are preserved and
their manifests differ because AssetRipper generates fresh GUID and
`timeCreated` values. Normalization replaces generated asset GUIDs with
path-derived identifiers, replaces other generated 32-character identifiers by
stable occurrence order within each file, and zeroes `timeCreated`.

- Exported files: {len(manifest_rows)}
- Serialized scenes: {scene_count}
- Resources maps: {map_count}
- Run A export manifest SHA-256: `{sha256_file(manifest_a)}`
- Run B export manifest SHA-256: `{sha256_file(manifest_b)}`
- Tool archive SHA-256: `680890b97c58be02075438814b3f41ec2a928ae0be8e94fa24ad51faa81078aa`

Both 313 MB exports remain outside Git under
`../good_pirate_1_13_evidence/asset-ripper`.
"""
    (evidence / "asset-ripper-determinism.md").write_text(
        report, encoding="utf-8", newline="\n"
    )
    print(
        "ASSETRIPPER_DETERMINISM_OK "
        f"files={len(manifest_rows)} normalized_view=identical"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())
