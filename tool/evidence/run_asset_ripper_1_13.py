#!/usr/bin/env python3
"""Run pinned AssetRipper against one clean 1.13 extraction."""

from __future__ import annotations

import argparse
import csv
import hashlib
import os
from pathlib import Path
import re
import shutil
import signal
import subprocess
import sys
import time
import urllib.parse
import urllib.request
import zipfile


DEFAULT_EXTERNAL_ROOT = Path("../good_pirate_1_13_evidence")
ASSETRIPPER_VERSION = "1.3.14"
ASSETRIPPER_URL = (
    "https://github.com/AssetRipper/AssetRipper/releases/download/1.3.14/"
    "AssetRipper_mac_arm64.zip"
)
ASSETRIPPER_ZIP_SHA256 = "680890b97c58be02075438814b3f41ec2a928ae0be8e94fa24ad51faa81078aa"
GUID_PATTERN = re.compile(r"(?<![0-9a-f])[0-9a-f]{32}(?![0-9a-f])")


def sha256_file(file_path: Path) -> str:
    digest = hashlib.sha256()
    with file_path.open("rb") as source:
        for chunk in iter(lambda: source.read(1024 * 1024), b""):
            digest.update(chunk)
    return digest.hexdigest()


def sha256_bytes(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def tsv_write(file_path: Path, header: list[str], rows: list[list[object]]) -> None:
    file_path.parent.mkdir(parents=True, exist_ok=True)
    with file_path.open("w", encoding="utf-8", newline="") as output:
        writer = csv.writer(output, delimiter="\t", lineterminator="\n")
        writer.writerow(header)
        writer.writerows(rows)


def ensure_asset_ripper(external_root: Path) -> tuple[Path, Path]:
    tools = external_root / "tools"
    package = tools / f"AssetRipper_mac_arm64_{ASSETRIPPER_VERSION}.zip"
    extracted = tools / f"AssetRipper-{ASSETRIPPER_VERSION}"
    executable = extracted / "AssetRipper.GUI.Free"
    tools.mkdir(parents=True, exist_ok=True)
    if not package.exists():
        temporary = package.with_suffix(".download")
        urllib.request.urlretrieve(ASSETRIPPER_URL, temporary)
        if sha256_file(temporary) != ASSETRIPPER_ZIP_SHA256:
            temporary.unlink(missing_ok=True)
            raise RuntimeError("Downloaded AssetRipper failed SHA-256 verification")
        os.replace(temporary, package)
    if sha256_file(package) != ASSETRIPPER_ZIP_SHA256:
        raise RuntimeError("Pinned AssetRipper package has an unexpected SHA-256")
    if not executable.exists():
        extracted.mkdir(parents=True, exist_ok=True)
        with zipfile.ZipFile(package) as archive:
            archive.extractall(extracted)
        executable.chmod(executable.stat().st_mode | 0o111)
    if not executable.is_file():
        raise RuntimeError("AssetRipper executable is missing")
    return package, executable


def post(port: int, endpoint: str, values: dict[str, str], timeout: int = 900) -> int:
    request = urllib.request.Request(
        f"http://127.0.0.1:{port}{endpoint}",
        data=urllib.parse.urlencode(values).encode("utf-8"),
        method="POST",
    )
    opener = urllib.request.build_opener(urllib.request.HTTPRedirectHandler())
    with opener.open(request, timeout=timeout) as response:
        return response.status


def wait_for_server(port: int, process: subprocess.Popen) -> None:
    for _ in range(200):
        if process.poll() is not None:
            raise RuntimeError(f"AssetRipper exited early with {process.returncode}")
        try:
            with urllib.request.urlopen(
                f"http://127.0.0.1:{port}/openapi.json", timeout=1
            ) as response:
                if response.status == 200:
                    return
        except Exception:
            time.sleep(0.1)
    raise RuntimeError("AssetRipper HTTP endpoint did not become ready")


def settings() -> dict[str, str]:
    return {
        "DefaultVersion": "0.0.0a0",
        "BundledAssetsExportMode": "DirectExport",
        "ScriptContentLevel": "Level2",
        "TargetVersion": "0.0.0a0",
        "AudioExportFormat": "Default",
        "ImageExportFormat": "Png",
        "LightmapTextureExportFormat": "Yaml",
        "SpriteExportMode": "Yaml",
        "ShaderExportMode": "Dummy",
        "TextExportMode": "Parse",
        "ScriptLanguageVersion": "CSharp7_3",
        "ScriptExportMode": "Decompiled",
    }


def export_project(
    executable: Path,
    input_directory: Path,
    export_directory: Path,
    log_path: Path,
    port: int,
) -> None:
    with log_path.open("wb") as log:
        process = subprocess.Popen(
            [str(executable), "--headless", "--port", str(port)],
            cwd=executable.parent,
            stdout=log,
            stderr=subprocess.STDOUT,
            start_new_session=True,
        )
        try:
            wait_for_server(port, process)
            post(port, "/Settings/Update", settings())
            post(port, "/LoadFolder", {"path": str(input_directory)})
            post(port, "/Export/UnityProject", {"path": str(export_directory)}, timeout=1800)
        finally:
            if process.poll() is None:
                process.send_signal(signal.SIGINT)
                try:
                    process.wait(timeout=15)
                except subprocess.TimeoutExpired:
                    process.kill()
                    process.wait(timeout=5)
    if not (export_directory / "ExportedProject/Assets").is_dir():
        raise RuntimeError("AssetRipper did not produce an ExportedProject/Assets directory")


def guid_normalization_map(export_directory: Path) -> dict[str, str]:
    mapping: dict[str, str] = {}
    for meta_path in sorted(export_directory.rglob("*.meta")):
        text = meta_path.read_text(encoding="utf-8", errors="replace")
        match = re.search(r"^guid: ([0-9a-f]{32})$", text, re.MULTILINE)
        if not match:
            continue
        asset_path = meta_path.relative_to(export_directory).as_posix()[:-5]
        stable = hashlib.sha256(asset_path.encode("utf-8")).hexdigest()[:32]
        mapping[match.group(1)] = stable
    return mapping


def normalized_file_bytes(file_path: Path, guid_map: dict[str, str]) -> bytes:
    raw = file_path.read_bytes()
    try:
        text = raw.decode("utf-8")
    except UnicodeDecodeError:
        return raw
    text = text.replace("\r\n", "\n").replace("\r", "\n")
    local_ids: dict[str, str] = {}

    def replace_identifier(match: re.Match[str]) -> str:
        identifier = match.group(0)
        if identifier in guid_map:
            return guid_map[identifier]
        if identifier not in local_ids:
            ordinal = len(local_ids)
            local_ids[identifier] = hashlib.sha256(
                f"local-identifier:{ordinal}".encode("ascii")
            ).hexdigest()[:32]
        return local_ids[identifier]

    text = GUID_PATTERN.sub(replace_identifier, text)
    text = re.sub(r"^timeCreated: \d+$", "timeCreated: 0", text, flags=re.MULTILINE)
    text = "\n".join(line.rstrip() for line in text.split("\n")).rstrip("\n") + "\n"
    return text.encode("utf-8")


def build_manifest(export_directory: Path, report_directory: Path) -> int:
    rows: list[list[object]] = []
    guid_map = guid_normalization_map(export_directory)
    files = sorted(
        [item for item in export_directory.rglob("*") if item.is_file()],
        key=lambda item: item.relative_to(export_directory).as_posix(),
    )
    for file_path in files:
        normalized = normalized_file_bytes(file_path, guid_map)
        rows.append(
            [
                file_path.relative_to(export_directory).as_posix(),
                file_path.stat().st_size,
                sha256_file(file_path),
                len(normalized),
                sha256_bytes(normalized),
            ]
        )
    tsv_write(
        report_directory / "asset-ripper-export-manifest.tsv",
        ["relative_path", "bytes", "sha256", "normalized_bytes", "normalized_sha256"],
        rows,
    )
    return len(files)


def build_census(
    export_directory: Path,
    report_directory: Path,
    log_path: Path,
    package: Path,
) -> None:
    assets = export_directory / "ExportedProject/Assets"
    guid_map = guid_normalization_map(export_directory)
    non_meta = sorted(
        [item for item in assets.rglob("*") if item.is_file() and item.suffix != ".meta"],
        key=lambda item: item.relative_to(assets).as_posix(),
    )
    groups: dict[str, list[Path]] = {}
    for file_path in non_meta:
        relative = file_path.relative_to(assets)
        category = relative.parts[0]
        groups.setdefault(category, []).append(file_path)
    rows: list[list[object]] = []
    for category in sorted(groups):
        members = groups[category]
        canonical_lines: list[str] = []
        for item in members:
            normalized = normalized_file_bytes(item, guid_map)
            canonical_lines.append(
                f"{item.relative_to(assets).as_posix()}\t{len(normalized)}\t{sha256_bytes(normalized)}\n"
            )
        canonical = "".join(canonical_lines).encode("utf-8")
        rows.append(
            [
                "asset_ripper_summary",
                "asset_ripper_" + category.lower().replace(" ", "_").replace(".", ""),
                category,
                len(members),
                sum(item.stat().st_size for item in members),
                0,
                sha256_bytes(canonical),
                f"ExportedProject/Assets/{category}",
                "verified_asset_ripper",
                "Non-meta files in the pinned 1.3.14 export",
            ]
        )
    for scene in sorted(assets.rglob("*.unity")):
        normalized = normalized_file_bytes(scene, guid_map)
        scene_text = normalized.decode("utf-8")
        rows.append(
            [
                "asset_ripper_item",
                "unity_scene",
                scene.stem,
                1,
                len(normalized),
                0,
                sha256_bytes(normalized),
                scene.relative_to(export_directory).as_posix(),
                "verified_asset_ripper",
                "Recovered serialized scene",
            ]
        )
        for structural_category, class_id in [
            ("scene_gameobjects", "1"),
            ("scene_monobehaviours", "114"),
        ]:
            count = len(re.findall(rf"^--- !u!{class_id} &", scene_text, re.MULTILINE))
            rows.append(
                [
                    "asset_ripper_structure",
                    structural_category,
                    scene.stem,
                    count,
                    0,
                    0,
                    "",
                    scene.relative_to(export_directory).as_posix(),
                    "verified_asset_ripper",
                    "Serialized Unity YAML object count",
                ]
            )
        rows.append(
            [
                "asset_ripper_structure",
                "scene_serialized_objects",
                scene.stem,
                len(re.findall(r"^--- !u!\d+ &", scene_text, re.MULTILINE)),
                0,
                0,
                "",
                scene.relative_to(export_directory).as_posix(),
                "verified_asset_ripper",
                "All serialized Unity YAML object records",
            ]
        )
    for category, pattern in [
        ("recovered_prefabs", "*.prefab"),
        ("recovered_materials", "*.mat"),
        ("recovered_animation_clips", "*.anim"),
        ("recovered_animator_controllers", "*.controller"),
        ("decompiled_cs_files", "*.cs"),
    ]:
        matching = sorted(assets.rglob(pattern))
        rows.append(
            [
                "asset_ripper_structure",
                category,
                category,
                len(matching),
                sum(item.stat().st_size for item in matching),
                0,
                "",
                f"ExportedProject/Assets/{pattern}",
                "verified_asset_ripper",
                "Recovered export file count",
            ]
        )
    map_root = assets / "Resources/mapdata"
    if map_root.is_dir():
        for map_file in sorted(map_root.glob("*.json")):
            normalized = normalized_file_bytes(map_file, guid_map)
            rows.append(
                [
                    "asset_ripper_item",
                    "resources_map",
                    map_file.stem,
                    1,
                    len(normalized),
                    0,
                    sha256_bytes(normalized),
                    map_file.relative_to(export_directory).as_posix(),
                    "verified_asset_ripper",
                    "Recovered Resources map",
                ]
            )
    log_text = log_path.read_text(encoding="utf-8", errors="replace")
    totals = [int(value) for value in re.findall(r"\((\d+)/(\d+)\) Exporting", log_text) for value in [value[1]]]
    if totals:
        rows.append(
            [
                "asset_ripper_summary",
                "asset_ripper_export_progress",
                "processed_assets",
                max(totals),
                0,
                0,
                "",
                "AssetRipper process log",
                "verified_asset_ripper",
                "Maximum AssetRipper export progress denominator",
            ]
        )
    rows.append(
        [
            "tool",
            "asset_ripper",
            ASSETRIPPER_VERSION,
            1,
            package.stat().st_size,
            0,
            sha256_file(package),
            package.name,
            "verified_tool",
            "Official mac arm64 release archive",
        ]
    )
    tsv_write(
        report_directory / "asset-ripper-census.tsv",
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


def parse_arguments() -> argparse.Namespace:
    parser = argparse.ArgumentParser()
    parser.add_argument("--external-root", type=Path, default=DEFAULT_EXTERNAL_ROOT)
    parser.add_argument("--run-id", required=True)
    parser.add_argument("--port", type=int, required=True)
    return parser.parse_args()


def main() -> int:
    arguments = parse_arguments()
    if not re.fullmatch(r"run-[a-z0-9]+", arguments.run_id):
        raise RuntimeError(f"Unsafe run id: {arguments.run_id}")
    external_root = arguments.external_root.resolve()
    input_directory = external_root / "runs" / arguments.run_id / "extracted"
    if not input_directory.is_dir():
        raise RuntimeError(f"Missing clean extraction: {input_directory}")
    package, executable = ensure_asset_ripper(external_root)
    export_directory = external_root / "asset-ripper" / arguments.run_id
    if export_directory.exists():
        shutil.rmtree(export_directory)
    export_directory.parent.mkdir(parents=True, exist_ok=True)
    report_directory = external_root / "asset-ripper-reports" / arguments.run_id
    if report_directory.exists():
        shutil.rmtree(report_directory)
    report_directory.mkdir(parents=True)
    log_path = report_directory / "asset-ripper.log"
    export_project(executable, input_directory, export_directory, log_path, arguments.port)
    file_count = build_manifest(export_directory, report_directory)
    build_census(export_directory, report_directory, log_path, package)
    print(
        f"ASSETRIPPER_RUN_OK run={arguments.run_id} files={file_count} "
        f"root={export_directory}"
    )
    return 0


if __name__ == "__main__":
    sys.exit(main())
