#!/usr/bin/env python3
"""Validate the reconstruction ledgers as strict tab-separated interfaces."""

from __future__ import annotations

import csv
import io
import re
from pathlib import Path

from common import REPO_ROOT, Results, load_config


ID_PATTERN = re.compile(r"^[A-Za-z0-9][A-Za-z0-9_.:-]*$")


def validate_ledger(results: Results, relative_path: str, expected_header: list[str]) -> None:
    path = REPO_ROOT / relative_path
    check_prefix = "LEDGER_" + Path(relative_path).stem.upper().replace("-", "_")
    if not path.is_file() or path.is_symlink():
        results.fail(f"{check_prefix}_PRESENT", f"missing regular file: {relative_path}")
        return
    results.pass_(f"{check_prefix}_PRESENT", relative_path)

    raw = path.read_bytes()
    if raw.startswith(b"\xef\xbb\xbf"):
        results.fail(f"{check_prefix}_ENCODING", "UTF-8 BOM is forbidden")
        return
    try:
        text = raw.decode("utf-8")
    except UnicodeDecodeError as error:
        results.fail(f"{check_prefix}_ENCODING", f"invalid UTF-8: {error}")
        return
    if b"\r" in raw:
        results.fail(f"{check_prefix}_ENCODING", "CR or CRLF line endings found")
    elif raw and not raw.endswith(b"\n"):
        results.fail(f"{check_prefix}_ENCODING", "file has no final LF")
    else:
        results.pass_(f"{check_prefix}_ENCODING", "UTF-8, LF-only, final newline")

    try:
        rows = list(csv.reader(io.StringIO(text, newline=""), delimiter="\t", strict=True))
    except csv.Error as error:
        results.fail(f"{check_prefix}_PARSE", f"TSV parse error: {error}")
        return
    if not rows:
        results.fail(f"{check_prefix}_SCHEMA", "ledger is empty and has no header")
        return
    if rows[0] != expected_header:
        results.fail(
            f"{check_prefix}_SCHEMA",
            f"expected header {expected_header}, observed {rows[0]}",
        )
    else:
        results.pass_(f"{check_prefix}_SCHEMA", f"{len(expected_header)} exact columns")

    width_errors: list[str] = []
    blank_rows: list[str] = []
    duplicate_ids: list[str] = []
    invalid_ids: list[str] = []
    seen_ids: set[str] = set()
    for line_number, row in enumerate(rows[1:], start=2):
        if not row or all(not cell for cell in row):
            blank_rows.append(str(line_number))
            continue
        if len(row) != len(expected_header):
            width_errors.append(f"line {line_number} has {len(row)} columns")
            continue
        row_id = row[0]
        if not ID_PATTERN.fullmatch(row_id):
            invalid_ids.append(f"line {line_number}: {row_id!r}")
        elif row_id in seen_ids:
            duplicate_ids.append(row_id)
        else:
            seen_ids.add(row_id)

    if width_errors or blank_rows:
        details = width_errors + ([f"blank lines: {', '.join(blank_rows)}"] if blank_rows else [])
        results.fail(f"{check_prefix}_ROWS", " | ".join(details))
    else:
        results.pass_(f"{check_prefix}_ROWS", f"{len(rows) - 1} data rows have fixed width")

    if invalid_ids or duplicate_ids:
        details = []
        if invalid_ids:
            details.append(f"invalid IDs: {', '.join(invalid_ids)}")
        if duplicate_ids:
            details.append(f"duplicate IDs: {', '.join(sorted(set(duplicate_ids)))}")
        results.fail(f"{check_prefix}_IDS", " | ".join(details))
    else:
        results.pass_(f"{check_prefix}_IDS", f"{len(seen_ids)} unique data-row IDs")


def main() -> int:
    results = Results("ledger schemas")
    ledgers = load_config()["ledgers"]
    for relative_path, expected_header in ledgers.items():
        validate_ledger(results, relative_path, expected_header)
    return results.finish()


if __name__ == "__main__":
    raise SystemExit(main())
