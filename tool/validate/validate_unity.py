#!/usr/bin/env python3
"""Validate the pinned Unity project identity and release-relevant settings."""

from __future__ import annotations

import json
import hashlib
import re
import sys
from pathlib import Path

from common import REPO_ROOT, Results, check_mode, load_config


def scalar(text: str, key: str) -> str | None:
    match = re.search(rf"^  {re.escape(key)}:[ \t]*(.*?)[ \t]*$", text, re.MULTILINE)
    return match.group(1) if match else None


def yaml_map(text: str, key: str) -> dict[str, str] | None:
    match = re.search(
        rf"^  {re.escape(key)}:\s*\n(?P<block>(?:    [^\n]*\n?)*)",
        text,
        re.MULTILINE,
    )
    if match is None:
        return None
    values: dict[str, str] = {}
    for line in match.group("block").splitlines():
        entry = re.fullmatch(r"    ([^:]+):\s*(.*?)\s*", line)
        if entry:
            values[entry.group(1)] = entry.group(2)
    return values


def exact_scalar(results: Results, text: str, key: str, expected: str, check_id: str) -> None:
    observed = scalar(text, key)
    if observed == expected:
        results.pass_(check_id, f"{key}={observed}")
    else:
        results.fail(check_id, f"expected {key}={expected}, observed {observed!r}")


def main() -> int:
    mode = check_mode(sys.argv)
    expected = load_config()["unity"]
    results = Results("Unity project")
    version_path = REPO_ROOT / "ProjectSettings" / "ProjectVersion.txt"
    if not version_path.exists():
        results.skip("UNITY_PROJECT_PRESENT", "ProjectVersion.txt is not present yet")
        return results.finish()
    results.pass_("UNITY_PROJECT_PRESENT", "Unity project settings are present")

    version_text = version_path.read_text(encoding="utf-8")
    editor_match = re.search(r"^m_EditorVersion:\s*(\S+)\s*$", version_text, re.MULTILINE)
    observed_editor = editor_match.group(1) if editor_match else None
    if observed_editor == expected["editor_version"]:
        results.pass_("UNITY_EDITOR_VERSION", observed_editor)
    else:
        results.fail(
            "UNITY_EDITOR_VERSION",
            f"expected {expected['editor_version']}, observed {observed_editor!r}",
        )

    settings_path = REPO_ROOT / "ProjectSettings" / "ProjectSettings.asset"
    if not settings_path.exists():
        results.fail("UNITY_PLAYER_SETTINGS", "ProjectSettings.asset is missing")
        return results.finish()
    settings = settings_path.read_text(encoding="utf-8")
    exact_scalar(results, settings, "companyName", expected["company_name"], "UNITY_COMPANY_NAME")
    exact_scalar(results, settings, "productName", expected["product_name"], "UNITY_PRODUCT_NAME")
    exact_scalar(results, settings, "bundleVersion", expected["version"], "UNITY_BUNDLE_VERSION")

    identifiers = yaml_map(settings, "applicationIdentifier")
    required_platforms = ("Android", "Standalone", "WebGL")
    if identifiers is None:
        results.fail("UNITY_APPLICATION_IDENTIFIER", "applicationIdentifier block is missing")
    else:
        mismatches = [
            f"{platform}={identifiers.get(platform)!r}"
            for platform in required_platforms
            if identifiers.get(platform) != expected["application_identifier"]
        ]
        if mismatches:
            results.fail("UNITY_APPLICATION_IDENTIFIER", ", ".join(mismatches))
        else:
            results.pass_(
                "UNITY_APPLICATION_IDENTIFIER",
                f"Android, Standalone, and WebGL use {expected['application_identifier']}",
            )

    display_expectations = {
        "defaultScreenHeight": str(expected["portrait_height"]),
        "defaultScreenHeightWeb": str(expected["portrait_height"]),
        "defaultScreenOrientation": "1",
        "defaultScreenWidth": str(expected["portrait_width"]),
        "defaultScreenWidthWeb": str(expected["portrait_width"]),
        "resizableWindow": "0",
        "runInBackground": "0",
    }
    display_errors = [
        f"{key}={scalar(settings, key)!r}"
        for key, value in display_expectations.items()
        if scalar(settings, key) != value
    ]
    if display_errors:
        results.fail("UNITY_PORTRAIT_SETTINGS", ", ".join(display_errors))
    else:
        results.pass_(
            "UNITY_PORTRAIT_SETTINGS",
            f"fixed {expected['portrait_width']}x{expected['portrait_height']} portrait player settings",
        )

    exact_scalar(
        results,
        settings,
        "activeInputHandler",
        "0",
        "UNITY_LEGACY_INPUT",
    )
    exact_scalar(
        results,
        settings,
        "useDeterministicCompilation",
        "1",
        "UNITY_DETERMINISTIC_COMPILATION",
    )
    exact_scalar(
        results,
        settings,
        "webGLTemplate",
        "PROJECT:GoodPirate113",
        "UNITY_WEBGL_TEMPLATE",
    )
    exact_scalar(
        results,
        settings,
        "AndroidMinSdkVersion",
        str(expected["android_min_sdk"]),
        "UNITY_ANDROID_MIN_SDK",
    )

    target_sdk = scalar(settings, "AndroidTargetSdkVersion")
    if target_sdk is not None and target_sdk.isdigit() and int(target_sdk) > 0:
        results.pass_("UNITY_ANDROID_TARGET_SDK", f"explicit target SDK {target_sdk}")
    else:
        results.fail("UNITY_ANDROID_TARGET_SDK", f"target SDK must be explicit and nonzero, observed {target_sdk!r}")

    exact_scalar(
        results,
        settings,
        "AndroidTargetArchitectures",
        str(expected["android_target_architectures"]),
        "UNITY_ANDROID_ARCHITECTURE",
    )

    backends = yaml_map(settings, "scriptingBackend")
    if backends is None:
        results.fail("UNITY_SCRIPTING_BACKEND", "scriptingBackend block is missing")
    else:
        mismatches = [
            f"{platform}={backends.get(platform)!r}"
            for platform in ("Android", "WebGL")
            if backends.get(platform) != "1"
        ]
        if mismatches:
            results.fail("UNITY_SCRIPTING_BACKEND", ", ".join(mismatches))
        else:
            results.pass_("UNITY_SCRIPTING_BACKEND", "Android and WebGL use IL2CPP")

    editor_settings_path = REPO_ROOT / "ProjectSettings" / "EditorSettings.asset"
    if editor_settings_path.exists():
        editor_settings = editor_settings_path.read_text(encoding="utf-8")
        visible_meta = scalar(editor_settings, "m_ExternalVersionControlSupport") == "Visible Meta Files"
        force_text = scalar(editor_settings, "m_SerializationMode") == "2"
        if visible_meta and force_text:
            results.pass_("UNITY_SERIALIZATION", "visible meta files and force-text serialization")
        else:
            results.fail("UNITY_SERIALIZATION", "visible meta files and force-text serialization are required")
    else:
        results.fail("UNITY_SERIALIZATION", "EditorSettings.asset is missing")

    manifest_path = REPO_ROOT / "Packages" / "manifest.json"
    lock_path = REPO_ROOT / "Packages" / "packages-lock.json"
    manifest_dependencies: dict[str, str] | None = None
    if not manifest_path.exists():
        results.fail("UNITY_PACKAGE_MANIFEST", "Packages/manifest.json is missing")
    else:
        try:
            manifest_document = json.loads(manifest_path.read_text(encoding="utf-8"))
        except (json.JSONDecodeError, UnicodeDecodeError) as error:
            results.fail("UNITY_PACKAGE_MANIFEST", f"cannot parse manifest: {error}")
        else:
            required_dependencies = expected["required_manifest_dependencies"]
            dependencies = manifest_document.get("dependencies") if isinstance(manifest_document, dict) else None
            if not isinstance(dependencies, dict):
                results.fail("UNITY_PACKAGE_MANIFEST", "dependencies must be a JSON object")
            else:
                manifest_dependencies = dependencies
                dependency_errors = [
                    f"{package_name}={dependencies.get(package_name)!r}"
                    for package_name, version in required_dependencies.items()
                    if dependencies.get(package_name) != version
                ]
                dependency_errors.extend(
                    f"{package_name} has unpinned version {version!r}"
                    for package_name, version in dependencies.items()
                    if not isinstance(version, str) or not version or version in {"latest", "*"}
                )
                if dependency_errors:
                    results.fail(
                        "UNITY_PACKAGE_MANIFEST",
                        " | ".join(dependency_errors),
                    )
                else:
                    results.pass_("UNITY_PACKAGE_MANIFEST", f"{len(dependencies)} pinned direct dependencies")

    if not lock_path.exists():
        results.fail("UNITY_PACKAGE_LOCK", "Packages/packages-lock.json is missing")
    else:
        try:
            lock_document = json.loads(lock_path.read_text(encoding="utf-8"))
        except (json.JSONDecodeError, UnicodeDecodeError) as error:
            results.fail("UNITY_PACKAGE_LOCK", f"cannot parse lock file: {error}")
        else:
            locked = lock_document.get("dependencies") if isinstance(lock_document, dict) else None
            if not isinstance(locked, dict):
                results.fail("UNITY_PACKAGE_LOCK", "dependencies must be a JSON object")
            elif manifest_dependencies is None:
                results.fail("UNITY_PACKAGE_LOCK", "cannot validate lock entries without a valid manifest")
            else:
                lock_errors = []
                for package_name, version in manifest_dependencies.items():
                    entry = locked.get(package_name)
                    if not isinstance(entry, dict):
                        lock_errors.append(f"missing {package_name}")
                    elif entry.get("version") != version or entry.get("depth") != 0:
                        lock_errors.append(f"{package_name}={entry}")
                if lock_errors:
                    results.fail("UNITY_PACKAGE_LOCK", " | ".join(lock_errors))
                else:
                    results.pass_("UNITY_PACKAGE_LOCK", "all direct packages have exact depth-zero lock entries")

    runtime_asmdef_path = (
        REPO_ROOT
        / "Assets"
        / "Reconstruction"
        / "Runtime"
        / "GoodPirate113.Reconstruction.Runtime.asmdef"
    )
    if not runtime_asmdef_path.exists():
        results.fail("UNITY_RUNTIME_ASSEMBLY", "runtime assembly definition is missing")
    else:
        try:
            runtime_asmdef = json.loads(runtime_asmdef_path.read_text(encoding="utf-8"))
        except (json.JSONDecodeError, UnicodeDecodeError) as error:
            results.fail("UNITY_RUNTIME_ASSEMBLY", f"cannot parse runtime assembly definition: {error}")
        else:
            if runtime_asmdef.get("name") == expected["runtime_assembly"]:
                results.pass_("UNITY_RUNTIME_ASSEMBLY", expected["runtime_assembly"])
            else:
                results.fail(
                    "UNITY_RUNTIME_ASSEMBLY",
                    f"expected {expected['runtime_assembly']}, observed {runtime_asmdef.get('name')!r}",
                )

    build_settings_path = REPO_ROOT / "ProjectSettings" / "EditorBuildSettings.asset"
    enabled_scenes: list[str] = []
    if build_settings_path.exists():
        build_settings = build_settings_path.read_text(encoding="utf-8")
        scene_entries = re.findall(
            r"- enabled:\s*1\s*\n\s+path:\s*(\S.*?)\s*\n",
            build_settings,
        )
        enabled_scenes = [entry for entry in scene_entries if entry]
    if enabled_scenes:
        results.pass_("UNITY_BUILD_SCENES", f"enabled scenes: {', '.join(enabled_scenes)}")
    elif mode == "release":
        results.fail("UNITY_BUILD_SCENES", "release candidate has no enabled build scene")
    else:
        results.warn("UNITY_BUILD_SCENES", "baseline has no enabled build scene yet")

    expected_shaders = {
        "Cartoon FX_Alpha Blended + Additive Soft.shader": (
            "f2263c2629ebab3a4e92a7a7f2cd7868b9daafbae01b4374031f86714793e2c4",
            "f008664108bbd3d45b128e3bcb3f582d",
        ),
        "Cartoon FX_Alpha Blended + Additive.shader": (
            "91b76b1af134306bdac311d0ec53ead568f10884c12aba49d0d9f74c28b6d9a4",
            "8a1b85dde83a7da4fa5efa18b27a1eeb",
        ),
        "Cartoon FX_Particle Multiply Colored.shader": (
            "a3f45df91920bd4f1e08967fe8fa181e87995e90bf77e75ecbb97980ed14047e",
            "86382a0d95cd4094096978b1d6869abb",
        ),
        "Cartoon FX_Particles Additive Alpha8.shader": (
            "217370bbe7599cdc16d060f3b384212176f11553ff6bafa4660a175c864d3b61",
            "0fea2fde9048bd74a903afd32c5a7339",
        ),
        "Cartoon FX_Particles Alpha Blended Alpha8.shader": (
            "b24e1a3a9e048160e1008d9a828b61cba6c2ea3c6f7f2d91a6a257503d0fa636",
            "9090ff3bc480c1244a926af4ba3cb2d6",
        ),
        "FX_Water.shader": (
            "471f8f8845d38b99fb3ecb16a32979c4b64efe519a9d7d9de14b0a2820e8aa0e",
            "930234d4435d6f848aaeff0db3f2216b",
        ),
        "Hidden_Cartoon FX_Particles.shader": (
            "ae1be265c47a1c4247f22194fa3398aefc2bf1e0c42bc47d4574f8c0d7b8e7e1",
            "0a49f750b63617544aecab62b5e0e766",
        ),
        "Mobile_DiffuseX.shader": (
            "5c5d95330221d7a1ff3e65786cd860087374db81b5a04ad9ed75dd870cd3dd72",
            "24d1ae10977aa374392da0af5684f05b",
        ),
    }
    shader_errors: list[str] = []
    for shader_name, (expected_hash, expected_guid) in expected_shaders.items():
        shader = REPO_ROOT / "Assets/Shader" / shader_name
        metadata_path = Path(str(shader) + ".meta")
        if not shader.is_file() or not metadata_path.is_file():
            shader_errors.append(f"missing {shader_name} or metadata")
            continue
        source_hash = hashlib.sha256(shader.read_bytes()).hexdigest()
        metadata = metadata_path.read_text(encoding="utf-8")
        guid_match = re.search(r"^guid:\s*([0-9a-f]{32})\s*$", metadata, re.MULTILINE)
        guid = guid_match.group(1) if guid_match else None
        if source_hash != expected_hash or guid != expected_guid:
            shader_errors.append(
                f"{shader_name}: hash={source_hash}, guid={guid!r}"
            )
    if shader_errors:
        results.fail("UNITY_COMPATIBILITY_SHADERS", " | ".join(shader_errors))
    else:
        results.pass_(
            "UNITY_COMPATIBILITY_SHADERS",
            "all eight user-authorized DOKDO-derived implementations retain Good Pirate GUIDs",
        )

    return results.finish()


if __name__ == "__main__":
    raise SystemExit(main())
