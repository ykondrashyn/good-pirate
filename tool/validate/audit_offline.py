#!/usr/bin/env python3
"""Reject online integrations, public endpoints, and credential configuration."""

from __future__ import annotations

import json
import re
from pathlib import Path
from urllib.parse import urlsplit

from common import REPO_ROOT, Results, repository_files


TEXT_SUFFIXES = {
    ".asmdef",
    ".asset",
    ".cs",
    ".gradle",
    ".json",
    ".js",
    ".jslib",
    ".manifest",
    ".md",
    ".plist",
    ".prefab",
    ".properties",
    ".shader",
    ".txt",
    ".unity",
    ".xml",
    ".yaml",
    ".yml",
}
FORBIDDEN_CONFIG_NAMES = {
    ".env",
    "googleservice-info.plist",
    "google-services.json",
    "keystore.properties",
}
FORBIDDEN_CONFIG_SUFFIXES = {".jks", ".keystore", ".p12", ".pem"}
FORBIDDEN_PACKAGE_MARKERS = (
    "adjust",
    "ads",
    "admob",
    "analytics",
    "appsflyer",
    "facebook",
    "firebase",
    "google-play-games",
    "iap",
    "playfab",
    "purchasing",
    "services.core",
    "services.cloud",
    "unitywebrequest",
)
FORBIDDEN_SOURCE_PATTERNS = {
    "BrowserNetwork": re.compile(r"\b(?:XMLHttpRequest|sendBeacon)\b|\bfetch\s*\("),
    "LegacyWWW": re.compile(r"\b(?:global::)?UnityEngine\.WWW\b"),
    "Socket": re.compile(r"\b(?:TcpClient|UdpClient|WebSocket|Socket)\b"),
    "SystemNetRequest": re.compile(r"\b(?:HttpClient|HttpWebRequest|WebClient|WebRequest)\b"),
    "UnityWebRequest": re.compile(r"\bUnityWebRequest\b"),
}
FORBIDDEN_ASSET_PATH_MARKERS = (
    "advertisement",
    "analytics",
    "appsflyer",
    "facebook",
    "firebase",
    "googleplaygames",
    "iapmanager",
    "mopub",
    "percentcloud",
    "purchasing",
)
URL_PATTERN = re.compile(r"\b(?:https?|wss?|ftp)://[^\s\"'<>]+", re.IGNORECASE)
DOMAIN_PATTERN = re.compile(
    r"(?<![A-Za-z0-9_-])(?:[A-Za-z0-9-]+\.)+(?:app|cloud|com|dev|io|net|org)(?![A-Za-z0-9_-])",
    re.IGNORECASE,
)


def relative(path: Path) -> str:
    return path.relative_to(REPO_ROOT).as_posix()


def is_local_host(host: str | None) -> bool:
    if host is None:
        return False
    normalized = host.lower().strip("[]")
    return normalized == "localhost" or normalized == "::1" or normalized.startswith("127.")


def readable_text(path: Path) -> str | None:
    if path.suffix.lower() not in TEXT_SUFFIXES or path.stat().st_size > 4 * 1024 * 1024:
        return None
    try:
        return path.read_text(encoding="utf-8")
    except UnicodeDecodeError:
        return None


def summarize(items: list[str], limit: int = 12) -> str:
    unique_items = sorted(set(items))
    files = {item.split(": ", 1)[0] for item in unique_items}
    examples = unique_items[:limit]
    detail = f"hits={len(unique_items)} files={len(files)}"
    if examples:
        detail += " | " + " | ".join(examples)
    remaining = len(unique_items) - len(examples)
    if remaining:
        detail += f" | and {remaining} more"
    return detail


def main() -> int:
    results = Results("offline boundary")
    scan_roots = [REPO_ROOT / name for name in ("Assets", "Packages", "ProjectSettings")]

    forbidden_configs: list[str] = []
    public_urls: list[str] = []
    naked_domains: list[str] = []
    source_apis: list[str] = []
    online_sdk_assets: list[str] = []
    for root in scan_roots:
        if not root.exists():
            continue
        for path in repository_files(root):
            if root.name == "Assets" and path.suffix.lower() != ".meta":
                lowered_path = relative(path).lower().replace("_", "")
                if any(marker in lowered_path for marker in FORBIDDEN_ASSET_PATH_MARKERS):
                    online_sdk_assets.append(relative(path))
            if path.name.lower() in FORBIDDEN_CONFIG_NAMES or path.suffix.lower() in FORBIDDEN_CONFIG_SUFFIXES:
                forbidden_configs.append(relative(path))
            text = readable_text(path)
            if text is None:
                continue
            urls = list(URL_PATTERN.finditer(text))
            for match in urls:
                url = match.group(0).rstrip(".,)")
                if not is_local_host(urlsplit(url).hostname):
                    public_urls.append(f"{relative(path)}: {url}")
            text_without_urls = URL_PATTERN.sub(" ", text)
            text_without_urls = re.sub(r"^%TAG.*$", "", text_without_urls, flags=re.MULTILINE)
            for domain in DOMAIN_PATTERN.findall(text_without_urls):
                lowered = domain.lower()
                if domain != lowered:
                    continue
                if lowered.startswith(("com.", "io.", "net.", "org.")):
                    continue
                naked_domains.append(f"{relative(path)}: {domain}")
            if root.name == "Assets" and path.suffix.lower() in {".cs", ".js", ".jslib"}:
                for label, pattern in FORBIDDEN_SOURCE_PATTERNS.items():
                    if pattern.search(text):
                        source_apis.append(f"{relative(path)}: {label}")

    if forbidden_configs:
        results.fail("OFFLINE_NO_SECRET_CONFIG", f"forbidden configuration files: {', '.join(sorted(forbidden_configs))}")
    else:
        results.pass_("OFFLINE_NO_SECRET_CONFIG", "no service, signing, or credential configuration files")

    endpoint_hits = sorted(set(public_urls + naked_domains))
    if endpoint_hits:
        results.fail("OFFLINE_NO_PUBLIC_ENDPOINTS", summarize(endpoint_hits))
    else:
        results.pass_("OFFLINE_NO_PUBLIC_ENDPOINTS", "no public URL or domain strings in runtime and project configuration")

    if source_apis:
        results.fail("OFFLINE_NO_NETWORK_APIS", summarize(source_apis))
    else:
        results.pass_("OFFLINE_NO_NETWORK_APIS", "no direct network API usage in reconstruction source")

    if online_sdk_assets:
        results.fail("OFFLINE_NO_ONLINE_SDK_ASSETS", summarize(online_sdk_assets))
    else:
        results.pass_("OFFLINE_NO_ONLINE_SDK_ASSETS", "no known online SDK asset paths")

    manifest_path = REPO_ROOT / "Packages" / "manifest.json"
    package_hits: list[str] = []
    if manifest_path.exists():
        try:
            dependencies = json.loads(manifest_path.read_text(encoding="utf-8")).get("dependencies", {})
            for package_name in dependencies:
                lowered = package_name.lower()
                if any(marker in lowered for marker in FORBIDDEN_PACKAGE_MARKERS):
                    package_hits.append(package_name)
        except (json.JSONDecodeError, UnicodeDecodeError) as error:
            results.fail("OFFLINE_PACKAGE_MANIFEST", f"cannot parse package manifest: {error}")
    if package_hits:
        results.fail("OFFLINE_PACKAGE_MANIFEST", f"online packages: {', '.join(sorted(package_hits))}")
    elif not any(item.check_id == "OFFLINE_PACKAGE_MANIFEST" for item in results.findings):
        results.pass_("OFFLINE_PACKAGE_MANIFEST", "no known online service package dependencies")

    project_settings_path = REPO_ROOT / "ProjectSettings" / "ProjectSettings.asset"
    if project_settings_path.exists():
        project_settings = project_settings_path.read_text(encoding="utf-8")
        enabled_flags = []
        for field in ("ForceInternetPermission", "submitAnalytics"):
            match = re.search(rf"^  {field}:\s*(\d+)\s*$", project_settings, re.MULTILINE)
            if match is None:
                enabled_flags.append(f"missing {field}")
            elif match.group(1) != "0":
                enabled_flags.append(f"{field}={match.group(1)}")
        cloud_id = re.search(r"^  cloudProjectId:[ \t]*(.*)$", project_settings, re.MULTILINE)
        if cloud_id is None:
            enabled_flags.append("missing cloudProjectId")
        elif cloud_id.group(1).strip():
            enabled_flags.append("cloudProjectId is nonempty")
        if enabled_flags:
            results.fail("OFFLINE_PLAYER_SETTINGS", ", ".join(enabled_flags))
        else:
            results.pass_("OFFLINE_PLAYER_SETTINGS", "internet permission, analytics, and cloud project are disabled")
    else:
        results.skip("OFFLINE_PLAYER_SETTINGS", "Unity PlayerSettings are not present yet")

    connect_path = REPO_ROOT / "ProjectSettings" / "UnityConnectSettings.asset"
    if connect_path.exists():
        connect_text = connect_path.read_text(encoding="utf-8")
        enabled_lines = [
            str(index)
            for index, line in enumerate(connect_text.splitlines(), start=1)
            if re.fullmatch(r"\s*m_Enabled:\s*1\s*", line)
            or re.fullmatch(r"\s*m_EnableCloudDiagnosticsReporting:\s*1\s*", line)
        ]
        if enabled_lines:
            results.fail("OFFLINE_UNITY_SERVICES", f"enabled Unity service flags at lines {', '.join(enabled_lines)}")
        else:
            results.pass_("OFFLINE_UNITY_SERVICES", "Unity Connect, analytics, ads, purchasing, and diagnostics are disabled")
    else:
        results.skip("OFFLINE_UNITY_SERVICES", "UnityConnectSettings are not present yet")

    return results.finish()


if __name__ == "__main__":
    raise SystemExit(main())
