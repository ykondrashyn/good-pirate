#!/usr/bin/env python3
"""Verify the authoritative APK without modifying or extracting it."""

from __future__ import annotations

import hashlib
import os
import stat
import sys
import zipfile
from pathlib import Path

from common import REPO_ROOT, Results, check_mode, is_relative_to, load_config


def immutable_flag_set(file_stat: os.stat_result) -> bool:
    flags = getattr(file_stat, "st_flags", 0)
    immutable_flags = getattr(stat, "UF_IMMUTABLE", 0) | getattr(stat, "SF_IMMUTABLE", 0)
    return bool(flags & immutable_flags)


def stable_identity(file_stat: os.stat_result) -> tuple[int, int, int, int, int]:
    return (
        file_stat.st_dev,
        file_stat.st_ino,
        file_stat.st_size,
        file_stat.st_mtime_ns,
        file_stat.st_ctime_ns,
    )


def main() -> int:
    mode = check_mode(sys.argv)
    config = load_config()["apk"]
    results = Results("authoritative APK")
    apk_path = Path(config["path"])

    if not apk_path.exists():
        results.fail("APK_PRESENT", f"missing authoritative APK at {apk_path}")
        return results.finish()
    if apk_path.is_symlink() or not apk_path.is_file():
        results.fail("APK_REGULAR_FILE", f"expected a regular, non-symlink file at {apk_path}")
        return results.finish()
    results.pass_("APK_PRESENT", f"regular file at {apk_path}")

    resolved_apk = apk_path.resolve()
    if is_relative_to(resolved_apk, REPO_ROOT.resolve()):
        results.fail("APK_EXTERNAL", "authoritative APK is embedded in the repository")
    else:
        results.pass_("APK_EXTERNAL", "authoritative APK is outside the repository")

    before = apk_path.stat()
    digest = hashlib.sha256()
    with apk_path.open("rb") as stream:
        opened = os.fstat(stream.fileno())
        if stable_identity(opened) != stable_identity(before):
            results.fail("APK_STABLE_READ", "file identity changed between stat and open")
        while chunk := stream.read(1024 * 1024):
            digest.update(chunk)

    actual_hash = digest.hexdigest()
    if actual_hash == config["sha256"]:
        results.pass_("APK_SHA256", actual_hash)
    else:
        results.fail(
            "APK_SHA256",
            f"expected {config['sha256']}, observed {actual_hash}",
        )

    if before.st_size == config["size_bytes"]:
        results.pass_("APK_SIZE", f"{before.st_size} bytes")
    else:
        results.fail(
            "APK_SIZE",
            f"expected {config['size_bytes']} bytes, observed {before.st_size}",
        )

    try:
        with zipfile.ZipFile(apk_path, "r") as archive:
            corrupt_member = archive.testzip()
        if corrupt_member is None:
            results.pass_("APK_ZIP_CRC", "all ZIP members passed CRC validation")
        else:
            results.fail("APK_ZIP_CRC", f"CRC failure in {corrupt_member}")
    except (OSError, zipfile.BadZipFile) as error:
        results.fail("APK_ZIP_CRC", f"could not validate archive: {error}")

    after = apk_path.stat()
    if stable_identity(after) == stable_identity(before):
        results.pass_("APK_STABLE_READ", "identity, size, mtime, and ctime stayed unchanged")
    else:
        results.fail("APK_STABLE_READ", "file metadata changed during validation")

    write_bits = stat.S_IWUSR | stat.S_IWGRP | stat.S_IWOTH
    permission_writable = bool(after.st_mode & write_bits)
    protected = immutable_flag_set(after) or not permission_writable
    if protected:
        results.pass_("APK_FILESYSTEM_IMMUTABLE", "owner writes are disabled or an immutable flag is set")
    elif mode == "release":
        results.fail("APK_FILESYSTEM_IMMUTABLE", "writable APK has no immutable filesystem flag")
    else:
        results.warn("APK_FILESYSTEM_IMMUTABLE", "writable APK has no immutable filesystem flag")

    return results.finish()


if __name__ == "__main__":
    raise SystemExit(main())
