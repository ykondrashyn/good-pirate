#!/usr/bin/env python3
"""Validate normalized, explicitly enumerated reconstruction inputs."""

from __future__ import annotations

import hashlib
import json
import subprocess
import sys
from pathlib import Path

from common import CONFIG_PATH, REPO_ROOT, Results, check_mode, git_output, load_config


class DuplicateJsonKey(ValueError):
    pass


def unique_object(pairs: list[tuple[str, object]]) -> dict[str, object]:
    result: dict[str, object] = {}
    for key, value in pairs:
        if key in result:
            raise DuplicateJsonKey(key)
        result[key] = value
    return result


def normalized_json(text: str) -> tuple[bool, str]:
    try:
        data = json.loads(text, object_pairs_hook=unique_object)
    except (json.JSONDecodeError, DuplicateJsonKey) as error:
        return False, str(error)
    expected = json.dumps(data, ensure_ascii=False, indent=2) + "\n"
    if text != expected:
        return False, "expected two-space JSON formatting with a final newline"
    return True, "valid normalized JSON with unique keys"


def summarize_paths(paths: list[str], limit: int = 12) -> str:
    unique_paths = sorted(set(paths))
    detail = f"count={len(unique_paths)}"
    examples = unique_paths[:limit]
    if examples:
        detail += " | " + " | ".join(examples)
    remaining = len(unique_paths) - len(examples)
    if remaining:
        detail += f" | and {remaining} more"
    return detail


def main() -> int:
    mode = check_mode(sys.argv)
    results = Results("reproducibility inputs")
    try:
        config = load_config()
    except (OSError, json.JSONDecodeError) as error:
        results.fail("REPRO_CONFIG", f"cannot read {CONFIG_PATH}: {error}")
        return results.finish()

    config_text = CONFIG_PATH.read_text(encoding="utf-8")
    canonical_config = json.dumps(config, ensure_ascii=False, indent=2, sort_keys=True) + "\n"
    if config_text == canonical_config:
        results.pass_("REPRO_CONFIG", "configuration is canonical JSON with sorted keys")
    else:
        results.fail("REPRO_CONFIG", "configuration is not canonical sorted JSON")

    input_paths = config.get("normalized_inputs")
    if not isinstance(input_paths, list) or not all(isinstance(item, str) for item in input_paths):
        results.fail("REPRO_INPUT_LIST", "normalized_inputs must be a string array")
        return results.finish()
    if input_paths != sorted(set(input_paths)):
        results.fail("REPRO_INPUT_LIST", "normalized_inputs must be sorted and unique")
    else:
        results.pass_("REPRO_INPUT_LIST", f"{len(input_paths)} sorted, unique paths")

    aggregate = hashlib.sha256()
    missing: list[str] = []
    encoding_errors: list[str] = []
    json_errors: list[str] = []
    for relative_path in input_paths:
        path = REPO_ROOT / relative_path
        if path.is_symlink() or not path.is_file():
            missing.append(relative_path)
            continue
        raw = path.read_bytes()
        aggregate.update(relative_path.encode("utf-8"))
        aggregate.update(b"\0")
        aggregate.update(hashlib.sha256(raw).digest())
        aggregate.update(b"\n")
        if raw.startswith(b"\xef\xbb\xbf"):
            encoding_errors.append(f"{relative_path}: UTF-8 BOM")
            continue
        try:
            text = raw.decode("utf-8")
        except UnicodeDecodeError as error:
            encoding_errors.append(f"{relative_path}: invalid UTF-8 at byte {error.start}")
            continue
        if b"\r" in raw:
            encoding_errors.append(f"{relative_path}: CR or CRLF line endings")
        if raw and not raw.endswith(b"\n"):
            encoding_errors.append(f"{relative_path}: no final LF")
        if path.suffix.lower() in {".asmdef", ".json"}:
            valid, detail = normalized_json(text)
            if not valid:
                json_errors.append(f"{relative_path}: {detail}")

    if missing:
        results.fail("REPRO_INPUT_PRESENCE", f"missing files: {', '.join(missing)}")
    else:
        results.pass_("REPRO_INPUT_PRESENCE", f"all {len(input_paths)} declared inputs exist")
    if encoding_errors:
        results.fail("REPRO_TEXT_NORMALIZATION", " | ".join(encoding_errors))
    else:
        results.pass_("REPRO_TEXT_NORMALIZATION", "all declared inputs are UTF-8, LF-only, and newline-terminated")
    if json_errors:
        results.fail("REPRO_JSON_NORMALIZATION", " | ".join(json_errors))
    else:
        results.pass_("REPRO_JSON_NORMALIZATION", "all declared JSON inputs have stable two-space formatting")
    results.pass_("REPRO_INPUT_DIGEST", aggregate.hexdigest())

    reference_inputs = config.get("reference_inputs")
    reference_failures: list[str] = []
    if not isinstance(reference_inputs, dict) or not reference_inputs:
        reference_failures.append("reference_inputs must be a nonempty object")
    else:
        for name, reference in sorted(reference_inputs.items()):
            if not isinstance(reference, dict):
                reference_failures.append(f"{name}: expected object")
                continue
            git_root = reference.get("git_root")
            commit = reference.get("commit")
            source_path = reference.get("path")
            expected_hash = reference.get("sha256")
            if not all(isinstance(value, str) and value for value in (
                git_root,
                commit,
                source_path,
                expected_hash,
            )):
                reference_failures.append(f"{name}: incomplete reference identity")
                continue
            process = subprocess.run(
                ["git", "-C", git_root, "show", f"{commit}:{source_path}"],
                check=False,
                capture_output=True,
            )
            if process.returncode != 0:
                detail = process.stderr.decode("utf-8", errors="replace").strip()
                reference_failures.append(f"{name}: {detail or 'git show failed'}")
                continue
            actual_hash = hashlib.sha256(process.stdout).hexdigest()
            if actual_hash != expected_hash:
                reference_failures.append(
                    f"{name}: expected {expected_hash}, found {actual_hash}"
                )
    if reference_failures:
        results.fail("REPRO_REFERENCE_INPUTS", " | ".join(reference_failures))
    else:
        results.pass_(
            "REPRO_REFERENCE_INPUTS",
            f"{len(reference_inputs)} external Git blobs match pinned commits and hashes",
        )

    gitignore_path = REPO_ROOT / ".gitignore"
    required_ignores = {
        "*.aab",
        "*.apk",
        "[Bb]uild/",
        "[Bb]uilds/",
        "[Ll]ibrary/",
        "[Ll]ogs/",
        "[Tt]emp/",
        "/decompiled/",
        "/evidence/",
        "/extracted/",
    }
    if not gitignore_path.exists():
        results.fail("REPRO_GITIGNORE", ".gitignore is missing")
    else:
        ignore_lines = {
            line.strip()
            for line in gitignore_path.read_text(encoding="utf-8").splitlines()
            if line.strip() and not line.lstrip().startswith("#")
        }
        missing_ignores = sorted(required_ignores - ignore_lines)
        if missing_ignores:
            results.fail("REPRO_GITIGNORE", f"missing exclusions: {', '.join(missing_ignores)}")
        else:
            results.pass_("REPRO_GITIGNORE", "build products, APKs, and recovery payloads are excluded")

    if mode == "release":
        tracked = git_output("ls-files", "--", *input_paths)
        if tracked.returncode != 0:
            results.fail("RELEASE_INPUTS_TRACKED", tracked.stderr.strip() or "git ls-files failed")
        else:
            tracked_paths = set(tracked.stdout.splitlines())
            untracked_inputs = [path for path in input_paths if path not in tracked_paths]
            if untracked_inputs:
                results.fail("RELEASE_INPUTS_TRACKED", summarize_paths(untracked_inputs))
            else:
                results.pass_("RELEASE_INPUTS_TRACKED", "all normalized inputs are tracked")

    return results.finish()


if __name__ == "__main__":
    raise SystemExit(main())
