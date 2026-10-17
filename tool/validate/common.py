#!/usr/bin/env python3
"""Shared helpers for the independent Good Pirate 1.13 validation gate."""

from __future__ import annotations

import hashlib
import json
import os
import subprocess
import sys
from dataclasses import dataclass
from pathlib import Path
from typing import Iterable


SCRIPT_DIR = Path(__file__).resolve().parent
REPO_ROOT = SCRIPT_DIR.parent.parent
CONFIG_PATH = REPO_ROOT / "docs" / "validation" / "reproducibility-inputs.json"
GENERATED_TOP_LEVEL_DIRS = {
    "Build",
    "Builds",
    "Library",
    "Logs",
    "MemoryCaptures",
    "Obj",
    "Temp",
    "UserSettings",
}


@dataclass(frozen=True)
class Finding:
    status: str
    check_id: str
    detail: str


class Results:
    """Collect findings and provide one stable, machine-readable exit result."""

    def __init__(self, title: str) -> None:
        self.title = title
        self.findings: list[Finding] = []

    def add(self, status: str, check_id: str, detail: str) -> None:
        finding = Finding(status, check_id, detail)
        self.findings.append(finding)
        print(f"{finding.status}\t{finding.check_id}\t{finding.detail}")

    def pass_(self, check_id: str, detail: str) -> None:
        self.add("PASS", check_id, detail)

    def fail(self, check_id: str, detail: str) -> None:
        self.add("FAIL", check_id, detail)

    def warn(self, check_id: str, detail: str) -> None:
        self.add("WARN", check_id, detail)

    def skip(self, check_id: str, detail: str) -> None:
        self.add("SKIP", check_id, detail)

    def finish(self) -> int:
        counts = {
            status: sum(item.status == status for item in self.findings)
            for status in ("PASS", "FAIL", "WARN", "SKIP")
        }
        print(
            "SUMMARY\t"
            f"{self.title}\t"
            f"pass={counts['PASS']} fail={counts['FAIL']} "
            f"warn={counts['WARN']} skip={counts['SKIP']}"
        )
        return 1 if counts["FAIL"] else 0


def load_config() -> dict:
    with CONFIG_PATH.open("r", encoding="utf-8") as stream:
        return json.load(stream)


def hash_file(path: Path, algorithm: str = "sha256") -> str:
    digest = hashlib.new(algorithm)
    with path.open("rb") as stream:
        while chunk := stream.read(1024 * 1024):
            digest.update(chunk)
    return digest.hexdigest()


def is_relative_to(path: Path, parent: Path) -> bool:
    try:
        path.relative_to(parent)
        return True
    except ValueError:
        return False


def repository_files(root: Path = REPO_ROOT) -> Iterable[Path]:
    """Yield tracked and untracked repository files, excluding Git internals."""

    for current_root, dir_names, file_names in os.walk(root, followlinks=False):
        current = Path(current_root)
        excluded = {".git", "__pycache__"}
        if current.resolve() == REPO_ROOT.resolve():
            excluded.update(GENERATED_TOP_LEVEL_DIRS)
        dir_names[:] = sorted(name for name in dir_names if name not in excluded)
        for file_name in sorted(file_names):
            yield current / file_name


def git_output(*args: str) -> subprocess.CompletedProcess[str]:
    return subprocess.run(
        ["git", *args],
        cwd=REPO_ROOT,
        check=False,
        capture_output=True,
        text=True,
    )


def read_text(path: Path) -> str:
    return path.read_text(encoding="utf-8")


def check_mode(argv: list[str]) -> str:
    mode = argv[1] if len(argv) > 1 else "baseline"
    if mode not in {"baseline", "release"}:
        print("usage: script [baseline|release]", file=sys.stderr)
        raise SystemExit(2)
    return mode
