#!/usr/bin/env python3
"""Audit the signed Good Pirate 1.13 Android checkpoint artifact."""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import subprocess
import sys
import tempfile
import zipfile
from pathlib import Path


EXPECTED_LIBRARIES = (
    "lib/arm64-v8a/libc++_shared.so",
    "lib/arm64-v8a/libil2cpp.so",
    "lib/arm64-v8a/libmain.so",
    "lib/arm64-v8a/libswappywrapper.so",
    "lib/arm64-v8a/libunity.so",
)
LEGACY_MARKERS = (
    b"appsflyer",
    b"facebook",
    b"googleanalytics",
    b"install_referrer",
    b"mopub",
    b"stansassets",
)


def run(*command: str) -> str:
    result = subprocess.run(
        command,
        check=False,
        stdout=subprocess.PIPE,
        stderr=subprocess.STDOUT,
        text=True,
    )
    if result.returncode:
        raise RuntimeError(
            f"command failed ({result.returncode}): {' '.join(command)}\n"
            f"{result.stdout}"
        )
    return result.stdout


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as stream:
        for block in iter(lambda: stream.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def value(pattern: str, text: str) -> str | None:
    match = re.search(pattern, text, re.MULTILINE)
    return match.group(1) if match else None


def scan_legacy_markers(archive: zipfile.ZipFile) -> list[dict[str, str]]:
    findings: list[dict[str, str]] = []
    overlap_size = max(map(len, LEGACY_MARKERS)) - 1
    for info in archive.infolist():
        if info.is_dir():
            continue
        found: set[bytes] = set()
        overlap = b""
        with archive.open(info) as stream:
            for block in iter(lambda: stream.read(1024 * 1024), b""):
                sample = (overlap + block).lower()
                found.update(marker for marker in LEGACY_MARKERS if marker in sample)
                overlap = sample[-overlap_size:]
        findings.extend(
            {"entry": info.filename, "marker": marker.decode("ascii")}
            for marker in sorted(found)
        )
    return findings


def build_report(args: argparse.Namespace) -> dict[str, object]:
    apk = args.apk.resolve()
    badging = run(str(args.aapt), "dump", "badging", str(apk))
    permissions_output = run(
        str(args.aapt), "dump", "permissions", str(apk)
    )
    manifest = run(
        str(args.aapt), "dump", "xmltree", str(apk),
        "AndroidManifest.xml"
    )
    signature = run(
        str(args.apksigner), "verify", "--verbose", "--print-certs",
        str(apk)
    )
    run(str(args.zipalign), "-c", "-P", "16", "-v", "4", str(apk))

    package_name = value(r"package: name='([^']+)'", badging)
    version_code = value(r"versionCode='([^']+)'", badging)
    version_name = value(r"versionName='([^']+)'", badging)
    minimum_api = value(r"^sdkVersion:'([^']+)'", badging)
    target_api = value(r"^targetSdkVersion:'([^']+)'", badging)
    native_code = value(r"^native-code: '([^']+)'", badging)
    gl_es = value(r"uses-gl-es: '([^']+)'", badging)
    permissions = sorted(set(re.findall(
        r"uses-permission: name='([^']+)'", permissions_output
    )))

    problems: list[str] = []
    expected_values = {
        "package": (package_name, "com.reconstruction.goodpirate113"),
        "version code": (version_code, "1130001"),
        "version name": (version_name, "1.13-r1"),
        "minimum API": (minimum_api, "25"),
        "target API": (target_api, "36"),
        "native code": (native_code, "arm64-v8a"),
        "OpenGL ES": (gl_es, "0x30000"),
    }
    for label, (actual, expected) in expected_values.items():
        if actual != expected:
            problems.append(
                f"{label} differs: expected {expected!r}, got {actual!r}"
            )
    if permissions or "android.permission." in manifest:
        problems.append("checkpoint manifest requests Android permissions")
    if "android:debuggable" in manifest:
        problems.append("checkpoint application is debuggable")
    if "com.unity3d.player.UnityPlayerActivity" not in manifest:
        problems.append("Unity Activity entry point is missing")
    if not re.search(
        r"android:screenOrientation\(0x0101001e\)=\(type 0x10\)0x1",
        manifest,
    ):
        problems.append("checkpoint Activity is not fixed to portrait")

    schemes = {
        scheme: value(
            rf"Verified using {re.escape(scheme)} scheme.*: (true|false)",
            signature,
        ) == "true"
        for scheme in ("v1", "v2", "v3", "v3.1", "v4")
    }
    if not schemes["v2"] or not schemes["v3"]:
        problems.append("APK is not signed with both v2 and v3 schemes")
    certificate_sha256 = value(
        r"Signer #1 certificate SHA-256 digest: ([0-9a-f]+)", signature
    )
    if not certificate_sha256:
        problems.append("signing certificate digest is missing")

    elf_alignments: dict[str, list[str]] = {}
    elf_machines: dict[str, str | None] = {}
    with zipfile.ZipFile(apk) as archive:
        libraries = sorted(
            name for name in archive.namelist()
            if re.fullmatch(r"lib/[^/]+/[^/]+\.so", name)
        )
        if tuple(libraries) != EXPECTED_LIBRARIES:
            problems.append("native library inventory is not exact ARM64")
        legacy_findings = scan_legacy_markers(archive)

        with tempfile.TemporaryDirectory(prefix="gp113-elf-") as directory:
            temporary = Path(directory)
            for library in libraries:
                extracted = temporary / Path(library).name
                with archive.open(library) as source:
                    extracted.write_bytes(source.read())
                headers = run(str(args.readelf), "-lW", str(extracted))
                file_header = run(str(args.readelf), "-h", str(extracted))
                alignments = [
                    line.split()[-1]
                    for line in headers.splitlines()
                    if line.lstrip().startswith("LOAD ")
                ]
                machine = value(r"^\s*Machine:\s+(.+)$", file_header)
                elf_alignments[library] = alignments
                elf_machines[library] = machine
                if not alignments or any(item != "0x4000" for item in alignments):
                    problems.append(f"ELF LOAD alignment differs: {library}")
                if machine != "AArch64":
                    problems.append(f"ELF machine is not AArch64: {library}")

    limitations = []
    if legacy_findings:
        limitations.append(
            "legacy SDK marker strings remain in the checkpoint payload"
        )
    limitations.append(
        "this checkpoint has not passed Android gameplay or parity acceptance"
    )

    return {
        "schema_version": 1,
        "classification": "reconstruction checkpoint, not a release candidate",
        "apk": {
            "name": apk.name,
            "bytes": apk.stat().st_size,
            "sha256": sha256(apk),
        },
        "android": {
            "package": package_name,
            "version_code": version_code,
            "version_name": version_name,
            "minimum_api": minimum_api,
            "target_api": target_api,
            "native_code": native_code,
            "gl_es": gl_es,
            "permissions": permissions,
            "debuggable": "android:debuggable" in manifest,
        },
        "signature": {
            "schemes": schemes,
            "certificate_sha256": certificate_sha256,
        },
        "alignment": {
            "zipalign_page_size": 16384,
            "zipalign_passed": True,
            "elf_load_segments": elf_alignments,
            "elf_machines": elf_machines,
        },
        "native_libraries": libraries,
        "legacy_payload_findings": legacy_findings,
        "limitations": limitations,
        "problems": sorted(set(problems)),
    }


def parse_args() -> argparse.Namespace:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--apk", type=Path, required=True)
    parser.add_argument("--aapt", type=Path, required=True)
    parser.add_argument("--apksigner", type=Path, required=True)
    parser.add_argument("--zipalign", type=Path, required=True)
    parser.add_argument("--readelf", type=Path, required=True)
    parser.add_argument("--output", type=Path, required=True)
    return parser.parse_args()


def main() -> int:
    args = parse_args()
    try:
        report = build_report(args)
    except (OSError, RuntimeError, zipfile.BadZipFile) as error:
        print(error, file=sys.stderr)
        return 1
    args.output.parent.mkdir(parents=True, exist_ok=True)
    args.output.write_text(
        json.dumps(report, indent=2, sort_keys=True) + "\n",
        encoding="utf-8",
    )
    print(
        "Good Pirate Android checkpoint audit: "
        f"{report['android']['package']}, "
        f"{len(report['native_libraries'])} ARM64 libraries, "
        f"{len(report['problems'])} installability problems, "
        f"{len(report['limitations'])} known limitations"
    )
    for problem in report["problems"]:
        print(f"- {problem}", file=sys.stderr)
    return 1 if report["problems"] else 0


if __name__ == "__main__":
    raise SystemExit(main())
