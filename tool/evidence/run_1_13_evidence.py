#!/usr/bin/env python3
"""Create deterministic, read-only recovery evidence for Good Pirate 1.13."""

from __future__ import annotations

import argparse
import base64
import csv
import hashlib
import json
import os
from pathlib import Path, PurePosixPath
import re
import shutil
import subprocess
import sys
import urllib.request
import zipfile


EXPECTED_APK_SHA256 = "eb30d068b2f811d5360b1d68dbb88d3969e4159ca3fd87223125a8b49e1773f7"
EXPECTED_APK_BYTES = 59_381_057
DEFAULT_APK = Path("../good_pirate_1_13_evidence/Good+Pirate_1.13_APKPure.apk")
DEFAULT_EXTERNAL_ROOT = Path("../good_pirate_1_13_evidence")
UNITY_ROOT = Path("/Applications/Unity/Hub/Editor/6000.3.18f1")
DOTNET_SDK_VERSION = "10.0.100"
ILSPY_VERSION = "11.0.0.9375"
ILSPY_NUPKG_SHA256 = "8f555b3fca90a1d7a59050d78539c69deedaad421756bd4fe478d135bdac2dea"
ILSPY_URL = (
    "https://api.nuget.org/v3-flatcontainer/ilspycmd/"
    f"{ILSPY_VERSION}/ilspycmd.{ILSPY_VERSION}.nupkg"
)


def sha256_file(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def tsv_write(path: Path, header: list[str], rows: list[list[object]]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    with path.open("w", encoding="utf-8", newline="") as output:
        writer = csv.writer(output, delimiter="\t", lineterminator="\n")
        writer.writerow(header)
        writer.writerows(rows)


def command(
    arguments: list[str],
    *,
    environment: dict[str, str] | None = None,
    working_directory: Path | None = None,
    timeout: int = 300,
    text: bool = True,
) -> subprocess.CompletedProcess:
    return subprocess.run(
        arguments,
        check=True,
        capture_output=True,
        cwd=working_directory,
        env=environment,
        timeout=timeout,
        text=text,
    )


def verify_exact_apk(apk: Path) -> None:
    if not apk.is_file():
        raise RuntimeError(f"APK not found: {apk}")
    size = apk.stat().st_size
    digest = sha256_file(apk)
    if size != EXPECTED_APK_BYTES or digest != EXPECTED_APK_SHA256:
        raise RuntimeError(
            "Refusing non-authoritative APK: "
            f"bytes={size}, sha256={digest}, expected_bytes={EXPECTED_APK_BYTES}, "
            f"expected_sha256={EXPECTED_APK_SHA256}"
        )


def safe_run_directory(external_root: Path, run_id: str) -> Path:
    if not re.fullmatch(r"run-[a-z0-9]+", run_id):
        raise RuntimeError(f"Unsafe run id: {run_id}")
    external_root = external_root.resolve()
    run_root = (external_root / "runs" / run_id).resolve()
    if run_root.parent != (external_root / "runs").resolve():
        raise RuntimeError(f"Unsafe run path: {run_root}")
    if run_root.exists():
        shutil.rmtree(run_root)
    run_root.mkdir(parents=True)
    return run_root


def dotnet_environment(external_root: Path) -> dict[str, str]:
    environment = os.environ.copy()
    environment.update(
        {
            "DOTNET_ROOT": str(external_root / "tools" / f"dotnet-{DOTNET_SDK_VERSION}"),
            "DOTNET_CLI_HOME": str(external_root / "tools" / "dotnet-cli-home"),
            "DOTNET_NOLOGO": "1",
            "DOTNET_SKIP_FIRST_TIME_EXPERIENCE": "1",
            "DOTNET_CLI_TELEMETRY_OPTOUT": "1",
            "NUGET_XMLDOC_MODE": "skip",
        }
    )
    return environment


def ensure_dotnet(external_root: Path) -> Path:
    executable = external_root / "tools" / f"dotnet-{DOTNET_SDK_VERSION}" / "dotnet"
    if not executable.is_file():
        raise RuntimeError(
            f"Pinned .NET SDK {DOTNET_SDK_VERSION} is absent at {executable}. "
            "Install it into the external evidence tools directory before running."
        )
    result = command(
        [str(executable), "--version"],
        environment=dotnet_environment(external_root),
    )
    if result.stdout.strip() != DOTNET_SDK_VERSION:
        raise RuntimeError(
            f"Unexpected .NET SDK version: {result.stdout.strip()}, "
            f"expected {DOTNET_SDK_VERSION}"
        )
    return executable


def ensure_ilspy(external_root: Path, dotnet: Path) -> tuple[Path, Path]:
    tools_root = external_root / "tools"
    package = tools_root / f"ilspycmd.{ILSPY_VERSION}.nupkg"
    manifest_root = tools_root / f"ilspycmd-{ILSPY_VERSION}-manifest"
    manifest = manifest_root / ".config" / "dotnet-tools.json"
    tools_root.mkdir(parents=True, exist_ok=True)
    if not package.exists():
        temporary = package.with_suffix(".download")
        urllib.request.urlretrieve(ILSPY_URL, temporary)
        if sha256_file(temporary) != ILSPY_NUPKG_SHA256:
            temporary.unlink(missing_ok=True)
            raise RuntimeError("Downloaded ILSpy package failed SHA-256 verification")
        os.replace(temporary, package)
    if sha256_file(package) != ILSPY_NUPKG_SHA256:
        raise RuntimeError("Pinned ILSpy package has an unexpected SHA-256")
    expected_manifest = {
        "version": 1,
        "isRoot": True,
        "tools": {
            "ilspycmd": {
                "version": ILSPY_VERSION,
                "commands": ["ilspycmd"],
                "rollForward": False,
            }
        },
    }
    if manifest.exists():
        if json.loads(manifest.read_text(encoding="utf-8")) != expected_manifest:
            raise RuntimeError(f"Unexpected ILSpy tool manifest: {manifest}")
    else:
        manifest.parent.mkdir(parents=True, exist_ok=True)
        manifest.write_text(
            json.dumps(expected_manifest, indent=2) + "\n", encoding="utf-8"
        )
    environment = dotnet_environment(external_root)
    command(
        [str(dotnet), "tool", "restore"],
        environment=environment,
        working_directory=manifest_root,
        timeout=600,
    )
    version = command(
        [str(dotnet), "tool", "run", "ilspycmd", "--", "--version"],
        environment=environment,
        working_directory=manifest_root,
    ).stdout
    if ILSPY_VERSION not in version:
        raise RuntimeError(f"Unexpected ILSpy version output: {version.strip()}")
    return package, manifest_root


def unity_tools(dotnet: Path) -> dict[str, Path]:
    scripting = UNITY_ROOT / "Unity.app/Contents/Resources/Scripting"
    mono_root = scripting / "MonoBleedingEdge"
    android = UNITY_ROOT / "PlaybackEngines/AndroidPlayer"
    values = {
        "mono": mono_root / "bin/mono",
        "csc": mono_root / "lib/mono/4.5/csc.exe",
        "ikdasm": mono_root / "lib/mono/4.5/ikdasm.exe",
        "cecil": mono_root
        / "lib/mono/gac/Mono.Cecil/0.11.1.0__0738eb9f132ed756/Mono.Cecil.dll",
        "dotnet": dotnet,
        "java_home": android / "OpenJDK",
        "apksigner": android / "SDK/build-tools/36.0.0/apksigner",
        "llvm_nm": android
        / "NDK/toolchains/llvm/prebuilt/darwin-x86_64/bin/llvm-nm",
    }
    missing = [str(path) for path in values.values() if not path.exists()]
    if missing:
        raise RuntimeError("Missing pinned Unity tools: " + ", ".join(missing))
    return values


def compile_managed_audit(
    repository_root: Path, external_root: Path, tools: dict[str, Path]
) -> Path:
    source = repository_root / "tool/evidence/ManagedAssemblyAudit.cs"
    executable = external_root / "tools/ManagedAssemblyAudit.exe"
    executable.parent.mkdir(parents=True, exist_ok=True)
    command(
        [
            str(tools["mono"]),
            str(tools["csc"]),
            "/nologo",
            "/target:exe",
            f"/out:{executable}",
            f"/reference:{tools['cecil']}",
            str(source),
        ]
    )
    return executable


def validated_zip_entries(archive: zipfile.ZipFile) -> list[zipfile.ZipInfo]:
    entries = archive.infolist()
    names: set[str] = set()
    for info in entries:
        path = PurePosixPath(info.filename)
        if info.filename in names:
            raise RuntimeError(f"Duplicate ZIP entry: {info.filename}")
        names.add(info.filename)
        if path.is_absolute() or ".." in path.parts:
            raise RuntimeError(f"Unsafe ZIP entry: {info.filename}")
    return sorted(entries, key=lambda info: info.filename)


def extract_and_manifest(apk: Path, run_root: Path) -> tuple[list[dict], Path]:
    extracted = run_root / "extracted"
    extracted.mkdir(parents=True)
    report_rows: list[list[object]] = []
    entries_data: list[dict] = []
    with zipfile.ZipFile(apk) as archive:
        entries = validated_zip_entries(archive)
        for info in entries:
            archive.extract(info, extracted)
            if info.is_dir():
                data = b""
            else:
                data = archive.read(info)
            digest = sha256_bytes(data)
            compression = {
                zipfile.ZIP_STORED: "stored",
                zipfile.ZIP_DEFLATED: "deflated",
                zipfile.ZIP_BZIP2: "bzip2",
                zipfile.ZIP_LZMA: "lzma",
            }.get(info.compress_type, str(info.compress_type))
            report_rows.append(
                [
                    info.filename,
                    info.file_size,
                    info.compress_size,
                    f"{info.CRC:08x}",
                    compression,
                    digest,
                ]
            )
            entries_data.append(
                {
                    "path": info.filename,
                    "bytes": info.file_size,
                    "compressed": info.compress_size,
                    "sha256": digest,
                    "directory": info.is_dir(),
                }
            )
    manifest = run_root / "reports/apk-entry-manifest.tsv"
    tsv_write(
        manifest,
        ["path", "bytes", "compressed_bytes", "crc32", "compression", "sha256"],
        report_rows,
    )
    return entries_data, extracted


def first_line(result: subprocess.CompletedProcess) -> str:
    return result.stdout.strip().splitlines()[0] if result.stdout.strip() else ""


def package_summary(
    apk: Path,
    extracted: Path,
    entries: list[dict],
    run_root: Path,
    tools: dict[str, Path],
    ilspy_package: Path,
    ilspy_manifest_root: Path,
    managed_audit_source: Path,
) -> None:
    environment = os.environ.copy()
    environment["JAVA_HOME"] = str(tools["java_home"])
    apkanalyzer = shutil.which("apkanalyzer")
    if not apkanalyzer:
        raise RuntimeError("apkanalyzer is unavailable")
    values: list[list[object]] = [
        ["apk.filename", apk.name, "filesystem"],
        ["apk.bytes", apk.stat().st_size, "filesystem"],
        ["apk.sha256", sha256_file(apk), "filesystem"],
    ]
    for key, operation in [
        ("package.application_id", "application-id"),
        ("package.version_code", "version-code"),
        ("package.version_name", "version-name"),
        ("package.min_sdk", "min-sdk"),
        ("package.target_sdk", "target-sdk"),
    ]:
        value = first_line(
            command(
                [apkanalyzer, "manifest", operation, str(apk)],
                environment=environment,
            )
        )
        values.append([key, value, f"apkanalyzer manifest {operation}"])
    signer = command(
        [str(tools["apksigner"]), "verify", "--print-certs", str(apk)],
        environment=environment,
    ).stdout
    signer_match = re.search(
        r"Signer #1 certificate SHA-256 digest: ([0-9a-fA-F]+)", signer
    )
    values.append(
        [
            "package.signer_sha256",
            signer_match.group(1).lower() if signer_match else "",
            "apksigner verify --print-certs",
        ]
    )
    manager_bytes = (extracted / "assets/bin/Data/globalgamemanagers").read_bytes()
    unity_match = re.search(rb"20\d\d\.\d+\.\d+[a-z]\d+", manager_bytes)
    unity_version = unity_match.group(0).decode("ascii") if unity_match else "unknown"
    names = [entry["path"] for entry in entries]
    abis = sorted(
        {
            PurePosixPath(name).parts[1]
            for name in names
            if name.startswith("lib/") and len(PurePosixPath(name).parts) >= 3
        }
    )
    managed_count = sum(
        1
        for name in names
        if name.startswith("assets/bin/Data/Managed/") and name.endswith(".dll")
    )
    has_mono = any(name.endswith("/libmonobdwgc-2.0.so") for name in names)
    has_il2cpp = any(name.endswith("/libil2cpp.so") for name in names)
    has_metadata = any(name.endswith("global-metadata.dat") for name in names)
    backend = "mono" if has_mono and not has_il2cpp and not has_metadata else "unknown"
    values.extend(
        [
            ["unity.version", unity_version, "globalgamemanagers string"],
            ["unity.scripting_backend", backend, "archive layout"],
            ["unity.abis", ",".join(abis), "archive layout"],
            ["managed.assembly_count", managed_count, "archive layout"],
            ["il2cpp.global_metadata_present", int(has_metadata), "archive layout"],
            ["tool.ilspy_version", ILSPY_VERSION, "pinned package"],
            ["tool.ilspy_nupkg_sha256", sha256_file(ilspy_package), "filesystem"],
            [
                "tool.ilspy_manifest_sha256",
                sha256_file(ilspy_manifest_root / ".config/dotnet-tools.json"),
                "filesystem",
            ],
            ["tool.dotnet_sdk_version", DOTNET_SDK_VERSION, "dotnet --version"],
            ["tool.dotnet_host_sha256", sha256_file(tools["dotnet"]), "filesystem"],
            ["tool.managed_audit_source_sha256", sha256_file(managed_audit_source), "filesystem"],
            ["tool.mono_cecil_sha256", sha256_file(tools["cecil"]), "filesystem"],
        ]
    )
    tsv_write(run_root / "reports/package-summary.tsv", ["key", "value", "evidence"], values)


def native_census(
    extracted: Path, entries: list[dict], run_root: Path, tools: dict[str, Path]
) -> None:
    file_tool = shutil.which("file")
    if not file_tool:
        raise RuntimeError("file tool is unavailable")
    rows: list[list[object]] = []
    native_entries = sorted(
        [entry for entry in entries if entry["path"].startswith("lib/") and entry["path"].endswith(".so")],
        key=lambda entry: entry["path"],
    )
    for entry in native_entries:
        relative = PurePosixPath(entry["path"])
        path = extracted.joinpath(*relative.parts)
        description = first_line(command([file_tool, "-b", str(path)]))
        symbol_output = command(
            [str(tools["llvm_nm"]), "-D", "--defined-only", str(path)], timeout=120
        ).stdout
        symbol_lines = [line for line in symbol_output.splitlines() if line.strip()]
        rows.append(
            [
                entry["path"],
                relative.parts[1],
                relative.name,
                entry["bytes"],
                entry["sha256"],
                len(symbol_lines),
                description,
                "ok",
            ]
        )
    tsv_write(
        run_root / "reports/native-library-census.tsv",
        [
            "path",
            "abi",
            "library",
            "bytes",
            "sha256",
            "dynamic_defined_symbols",
            "file_description",
            "status",
        ],
        rows,
    )


def managed_audit(
    extracted: Path,
    run_root: Path,
    executable: Path,
    tools: dict[str, Path],
) -> None:
    environment = os.environ.copy()
    environment["MONO_PATH"] = str(tools["cecil"].parent)
    reports = run_root / "reports"
    command(
        [
            str(tools["mono"]),
            str(executable),
            str(extracted / "assets/bin/Data/Managed"),
            str(reports / "managed-assembly-census.tsv"),
            str(reports / "mono-method-index.tsv"),
        ],
        environment=environment,
        timeout=600,
    )


def normalize_text(text: str, run_root: Path, extracted: Path) -> str:
    text = text.replace("\r\n", "\n").replace("\r", "\n")
    text = text.replace(str(run_root), "<RUN_ROOT>")
    text = text.replace(str(extracted), "<EXTRACTED_ROOT>")
    text = re.sub(r"// MVID: \{[0-9A-Fa-f-]+\}", "// MVID: <NORMALIZED>", text)
    lines = [line.rstrip() for line in text.split("\n")]
    return "\n".join(lines).rstrip("\n") + "\n"


def cil_dumps(
    extracted: Path, run_root: Path, tools: dict[str, Path]
) -> None:
    managed = extracted / "assets/bin/Data/Managed"
    destination = run_root / "cil"
    destination.mkdir(parents=True)
    rows: list[list[object]] = []
    for assembly in sorted(managed.glob("*.dll"), key=lambda path: path.name):
        output_path = destination / f"{assembly.name}.il"
        try:
            result = command(
                [str(tools["mono"]), str(tools["ikdasm"]), str(assembly)],
                timeout=300,
            )
            normalized = normalize_text(result.stdout, run_root, extracted)
            output_path.write_text(normalized, encoding="utf-8", newline="\n")
            rows.append(
                [
                    assembly.name,
                    sha256_file(assembly),
                    f"cil/{output_path.name}",
                    sha256_file(output_path),
                    output_path.stat().st_size,
                    "ok",
                    "",
                ]
            )
        except Exception as error:
            rows.append(
                [
                    assembly.name,
                    sha256_file(assembly),
                    "",
                    "",
                    0,
                    "error",
                    f"{type(error).__name__}: {error}",
                ]
            )
    tsv_write(
        run_root / "reports/normalized-cil-manifest.tsv",
        [
            "assembly",
            "assembly_sha256",
            "normalized_cil_path",
            "normalized_cil_sha256",
            "normalized_cil_bytes",
            "status",
            "error",
        ],
        rows,
    )


def normalize_source_tree(source_root: Path, run_root: Path, extracted: Path) -> None:
    for path in sorted(source_root.rglob("*")):
        if not path.is_file():
            continue
        try:
            original = path.read_text(encoding="utf-8")
        except UnicodeDecodeError:
            continue
        path.write_text(
            normalize_text(original, run_root, extracted), encoding="utf-8", newline="\n"
        )


def source_decompile(
    extracted: Path,
    run_root: Path,
    tools: dict[str, Path],
    ilspy_manifest_root: Path,
    external_root: Path,
) -> None:
    managed = extracted / "assets/bin/Data/Managed"
    source_root = run_root / "decompiled"
    rows: list[list[object]] = []
    for assembly_name in ["Assembly-CSharp.dll", "Assembly-CSharp-firstpass.dll"]:
        assembly = managed / assembly_name
        destination = source_root / assembly.stem
        destination.mkdir(parents=True)
        status = "ok"
        error = ""
        try:
            command(
                [
                    str(tools["dotnet"]),
                    "tool",
                    "run",
                    "ilspycmd",
                    "--",
                    "--disable-updatecheck",
                    "--nested-directories",
                    "-lv",
                    "CSharp7_3",
                    "-p",
                    "-o",
                    str(destination),
                    "-r",
                    str(managed),
                    str(assembly),
                ],
                environment=dotnet_environment(external_root),
                working_directory=ilspy_manifest_root,
                timeout=900,
            )
            normalize_source_tree(destination, run_root, extracted)
        except Exception as exception:
            status = "error"
            error = f"{type(exception).__name__}: {exception}"
        files = [path for path in destination.rglob("*") if path.is_file()]
        rows.append(
            [
                "assembly",
                assembly_name,
                f"decompiled/{assembly.stem}",
                "",
                sum(path.stat().st_size for path in files),
                len(files),
                status,
                error,
            ]
        )
        for path in sorted(files, key=lambda item: item.relative_to(run_root).as_posix()):
            rows.append(
                [
                    "file",
                    assembly_name,
                    path.relative_to(run_root).as_posix(),
                    sha256_file(path),
                    path.stat().st_size,
                    1,
                    "ok",
                    "",
                ]
            )
    tsv_write(
        run_root / "reports/source-manifest.tsv",
        [
            "record_type",
            "assembly",
            "path",
            "sha256",
            "bytes",
            "file_count",
            "status",
            "error",
        ],
        rows,
    )


def content_category(path: str) -> str:
    name = PurePosixPath(path).name
    if path == "AndroidManifest.xml":
        return "android_manifest"
    if re.fullmatch(r"classes\d*\.dex", name):
        return "android_dex"
    if path.startswith("lib/") and path.endswith(".so"):
        return "native_library"
    if path.startswith("assets/bin/Data/Managed/") and path.endswith(".dll"):
        return "managed_assembly"
    if re.fullmatch(r"assets/bin/Data/level\d+(?:\.split\d+)?", path):
        return "unity_scene_payload"
    if path.startswith("assets/bin/Data/globalgamemanagers"):
        return "unity_global_managers"
    if path.startswith("assets/bin/Data/") and path.endswith(".resource"):
        return "unity_resource_stream"
    if path.startswith("assets/bin/Data/Managed/"):
        return "managed_support"
    if path.startswith("assets/bin/Data/"):
        return "unity_serialized_data"
    if path.startswith("assets/"):
        return "android_asset"
    if path.startswith("res/") or path == "resources.arsc":
        return "android_resource"
    if path.startswith("META-INF/"):
        return "signature_metadata"
    return "package_support"


def content_census(entries: list[dict], run_root: Path) -> None:
    groups: dict[str, list[dict]] = {}
    for entry in entries:
        if entry["directory"]:
            continue
        groups.setdefault(content_category(entry["path"]), []).append(entry)
    rows: list[list[object]] = []
    for category in sorted(groups):
        members = sorted(groups[category], key=lambda entry: entry["path"])
        canonical = "".join(
            f"{entry['path']}\t{entry['bytes']}\t{entry['sha256']}\n" for entry in members
        ).encode("utf-8")
        rows.append(
            [
                "summary",
                category,
                category,
                len(members),
                sum(entry["bytes"] for entry in members),
                sum(entry["compressed"] for entry in members),
                sha256_bytes(canonical),
                "",
                "verified_archive",
                "Aggregate of exact APK entries",
            ]
        )
    detailed_categories = {
        "managed_assembly",
        "native_library",
        "unity_scene_payload",
        "unity_global_managers",
    }
    for category in sorted(detailed_categories):
        for entry in sorted(groups.get(category, []), key=lambda item: item["path"]):
            rows.append(
                [
                    "item",
                    category,
                    PurePosixPath(entry["path"]).name,
                    1,
                    entry["bytes"],
                    entry["compressed"],
                    entry["sha256"],
                    entry["path"],
                    "verified_archive",
                    "Exact 1.13 APK entry",
                ]
            )
    tsv_write(
        run_root / "reports/content-census.tsv",
        [
            "record_type",
            "category",
            "item_id",
            "count",
            "bytes",
            "compressed_bytes",
            "sha256",
            "source_path",
            "evidence_status",
            "notes",
        ],
        rows,
    )


def artifact_manifest(run_root: Path) -> None:
    rows: list[list[object]] = []
    for relative_root, artifact_class in [
        (run_root / "reports", "report"),
        (run_root / "cil", "normalized_cil"),
        (run_root / "decompiled", "decompiled_source"),
    ]:
        if not relative_root.exists():
            continue
        for path in sorted(
            [item for item in relative_root.rglob("*") if item.is_file()],
            key=lambda item: item.relative_to(run_root).as_posix(),
        ):
            if path.name == "external-artifact-manifest.tsv":
                continue
            rows.append(
                [
                    artifact_class,
                    path.relative_to(run_root).as_posix(),
                    path.stat().st_size,
                    sha256_file(path),
                ]
            )
    tsv_write(
        run_root / "reports/external-artifact-manifest.tsv",
        ["artifact_class", "relative_path", "bytes", "sha256"],
        rows,
    )


def parse_arguments() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--apk", type=Path, default=DEFAULT_APK)
    parser.add_argument("--external-root", type=Path, default=DEFAULT_EXTERNAL_ROOT)
    parser.add_argument("--run-id", required=True)
    return parser.parse_args()


def main() -> int:
    arguments = parse_arguments()
    repository_root = Path(__file__).resolve().parents[2]
    apk = arguments.apk.resolve()
    external_root = arguments.external_root.resolve()
    verify_exact_apk(apk)
    run_root = safe_run_directory(external_root, arguments.run_id)
    dotnet = ensure_dotnet(external_root)
    tools = unity_tools(dotnet)
    ilspy_package, ilspy_manifest_root = ensure_ilspy(external_root, dotnet)
    managed_audit_executable = compile_managed_audit(repository_root, external_root, tools)
    entries, extracted = extract_and_manifest(apk, run_root)
    package_summary(
        apk,
        extracted,
        entries,
        run_root,
        tools,
        ilspy_package,
        ilspy_manifest_root,
        repository_root / "tool/evidence/ManagedAssemblyAudit.cs",
    )
    native_census(extracted, entries, run_root, tools)
    managed_audit(extracted, run_root, managed_audit_executable, tools)
    cil_dumps(extracted, run_root, tools)
    source_decompile(extracted, run_root, tools, ilspy_manifest_root, external_root)
    content_census(entries, run_root)
    artifact_manifest(run_root)
    print(f"EVIDENCE_RUN_OK run={arguments.run_id} root={run_root}")
    return 0


if __name__ == "__main__":
    sys.exit(main())
