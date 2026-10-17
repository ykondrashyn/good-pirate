#!/usr/bin/env python3
"""Compare two 1.13 evidence runs and publish only deterministic indexes."""

from __future__ import annotations

import argparse
import csv
import hashlib
from pathlib import Path
import shutil
import sys


DEFAULT_EXTERNAL_ROOT = Path("../good_pirate_1_13_evidence")


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def read_tsv(path: Path) -> tuple[list[str], list[dict[str, str]]]:
    with path.open(encoding="utf-8", newline="") as source:
        reader = csv.DictReader(source, delimiter="\t")
        return list(reader.fieldnames or []), list(reader)


def require_success(path: Path, status_column: str = "status") -> None:
    _, rows = read_tsv(path)
    failures = [row for row in rows if row.get(status_column, "") not in {"", "ok"}]
    if failures:
        sample = failures[0]
        raise RuntimeError(f"Evidence failure in {path.name}: {sample}")


def compare_runs(run_a: Path, run_b: Path) -> tuple[str, int]:
    manifest_name = "reports/external-artifact-manifest.tsv"
    manifest_a = run_a / manifest_name
    manifest_b = run_b / manifest_name
    if manifest_a.read_bytes() != manifest_b.read_bytes():
        _, rows_a = read_tsv(manifest_a)
        _, rows_b = read_tsv(manifest_b)
        map_a = {row["relative_path"]: row for row in rows_a}
        map_b = {row["relative_path"]: row for row in rows_b}
        differences = []
        for key in sorted(set(map_a) | set(map_b)):
            if map_a.get(key) != map_b.get(key):
                differences.append(key)
        raise RuntimeError(
            "Evidence runs differ: " + ", ".join(differences[:20])
        )
    _, rows = read_tsv(manifest_a)
    for row in rows:
        relative = row["relative_path"]
        for root in (run_a, run_b):
            path = root / relative
            if not path.is_file():
                raise RuntimeError(f"Manifest references missing artifact: {path}")
            if str(path.stat().st_size) != row["bytes"]:
                raise RuntimeError(f"Size mismatch: {path}")
            if sha256_file(path) != row["sha256"]:
                raise RuntimeError(f"SHA-256 mismatch: {path}")
    return sha256_file(manifest_a), len(rows)


def copy_file(source: Path, destination: Path) -> None:
    destination.parent.mkdir(parents=True, exist_ok=True)
    shutil.copyfile(source, destination)


def publish(repository_root: Path, run_a: Path, manifest_sha: str, artifact_count: int) -> None:
    reports = run_a / "reports"
    require_success(reports / "managed-assembly-census.tsv")
    require_success(reports / "mono-method-index.tsv")
    require_success(reports / "normalized-cil-manifest.tsv")
    require_success(reports / "source-manifest.tsv")

    copy_file(reports / "mono-method-index.tsv", repository_root / "docs/mono-method-index.tsv")
    copy_file(reports / "content-census.tsv", repository_root / "docs/content-census.tsv")
    evidence_files = [
        "apk-entry-manifest.tsv",
        "package-summary.tsv",
        "native-library-census.tsv",
        "managed-assembly-census.tsv",
        "normalized-cil-manifest.tsv",
        "source-manifest.tsv",
        "external-artifact-manifest.tsv",
    ]
    for name in evidence_files:
        copy_file(reports / name, repository_root / "docs/evidence" / name)

    _, assembly_rows = read_tsv(reports / "managed-assembly-census.tsv")
    _, method_rows = read_tsv(reports / "mono-method-index.tsv")
    _, cil_rows = read_tsv(reports / "normalized-cil-manifest.tsv")
    _, source_rows = read_tsv(reports / "source-manifest.tsv")
    body_rows = [row for row in method_rows if row["has_body"] == "1"]
    nontrivial_rows = [
        row for row in method_rows if int(row["instruction_count"] or "0") > 1
    ]
    source_files = [row for row in source_rows if row["record_type"] == "file"]
    report = f"""# Good Pirate 1.13 evidence determinism

Two independent clean extractions of the exact APK produced identical normalized
evidence manifests.

- APK SHA-256: `eb30d068b2f811d5360b1d68dbb88d3969e4159ca3fd87223125a8b49e1773f7`
- External artifact manifest SHA-256: `{manifest_sha}`
- Compared external artifacts: {artifact_count}
- Managed assemblies: {len(assembly_rows)}
- Indexed managed methods: {len(method_rows)}
- Methods with CIL bodies: {len(body_rows)}
- Methods with more than one CIL instruction: {len(nontrivial_rows)}
- Normalized assembly CIL dumps: {len([row for row in cil_rows if row['status'] == 'ok'])}
- Decompiled source files: {len(source_files)}

The two run directories remain outside Git under
`../good_pirate_1_13_evidence/runs`. Bulky extracted files,
normalized CIL, and decompiled source are not copied into this repository.
"""
    (repository_root / "docs/evidence/determinism-report.md").write_text(
        report, encoding="utf-8", newline="\n"
    )


def parse_arguments() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--external-root", type=Path, default=DEFAULT_EXTERNAL_ROOT)
    parser.add_argument("--run-a", default="run-a")
    parser.add_argument("--run-b", default="run-b")
    return parser.parse_args()


def main() -> int:
    arguments = parse_arguments()
    repository_root = Path(__file__).resolve().parents[2]
    runs = arguments.external_root.resolve() / "runs"
    run_a = runs / arguments.run_a
    run_b = runs / arguments.run_b
    manifest_sha, artifact_count = compare_runs(run_a, run_b)
    publish(repository_root, run_a, manifest_sha, artifact_count)
    print(
        "EVIDENCE_DETERMINISM_OK "
        f"manifest_sha256={manifest_sha} artifacts={artifact_count}"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())
