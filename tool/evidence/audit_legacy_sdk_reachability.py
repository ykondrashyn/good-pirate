#!/usr/bin/env python3
"""Deterministically audit legacy SDK reachability in the Unity export.

The audit is intentionally conservative.  It proves static roots from serialized
GUIDs, persistent UnityEvents, project-owned source references, Resources and
reflection strings, and AddComponent calls.  Candidate-to-candidate references
are then closed transitively from those roots.  Absence from the report is not
proof that runtime reflection cannot reach an artifact.
"""

from __future__ import annotations

import argparse
import hashlib
import re
import subprocess
from collections import Counter, defaultdict, deque
from dataclasses import dataclass, field
from pathlib import Path


REPORT_SCHEMA = "good-pirate-1.13-legacy-sdk-reachability-v1"

SERIALIZED_SUFFIXES = {
    ".anim",
    ".asset",
    ".controller",
    ".mat",
    ".overrideController",
    ".playable",
    ".prefab",
    ".unity",
}
BINARY_SUFFIXES = {".a", ".aar", ".bundle", ".dll", ".jar", ".so"}

NETWORK_PATTERNS = {
    "url": re.compile(r"https?://", re.IGNORECASE),
    "unity-www": re.compile(r"\b(?:global::)?UnityEngine\.(?:WWW|Networking\.UnityWebRequest)\b"),
    "dotnet-network": re.compile(
        r"\b(?:global::)?System\.Net\.(?:Http|Sockets|WebRequest|HttpWebRequest|WebClient)\b"
    ),
    "socket": re.compile(r"\b(?:TcpClient|UdpClient|Socket)\b"),
    "native-bridge": re.compile(r"\b(?:DllImport|AndroidJavaClass|AndroidJavaObject)\b"),
}

FAMILY_RULES = (
    ("ads", re.compile(r"mopub|admob|unityads|chartboost|adcolony|advertis", re.IGNORECASE)),
    ("attribution", re.compile(r"appsflyer|adjust", re.IGNORECASE)),
    ("analytics", re.compile(r"analytics|googleanalytics|gaihandler", re.IGNORECASE)),
    ("billing", re.compile(r"purchas|billing|iapmanager|(?:^|[/_.])iap(?:[/_.]|$)|appstore|channelpurchase|facebookstore|unitystore", re.IGNORECASE)),
    ("cloud", re.compile(r"cloud|icloud|savedgame", re.IGNORECASE)),
    ("social", re.compile(r"facebook|googleplaygames|gamecenter|socialplatform|sharing|twitter", re.IGNORECASE)),
    ("network", re.compile(r"percent/http|httpsclient|textloader|textureloader|webrequest|network", re.IGNORECASE)),
)

KNOWN_SDK_PATH = re.compile(
    r"(?:"
    r"/GooglePlayGames/|/Com/Google/|/Facebook/Unity/|/MoPubInternal/|"
    r"/Percent/|/UnityEngine/Purchasing/|/SA/"
    r")",
    re.IGNORECASE,
)

KNOWN_OFFLINE_SHIMS = {
    "Assets/Scripts/Assembly-CSharp/Appsflyer.cs",
    "Assets/Scripts/Assembly-CSharp/GoogleCloudManager.cs",
    "Assets/Scripts/Assembly-CSharp/ICloudManager.cs",
    "Assets/Scripts/Assembly-CSharp/IapManager.cs",
    "Assets/Scripts/Assembly-CSharp/MoPubAdManager.cs",
    "Assets/Scripts/Assembly-CSharp/MoPubBinding.cs",
    "Assets/Scripts/Assembly-CSharp/MoPubiOS.cs",
    "Assets/Scripts/Assembly-CSharp/SocialPlatformManager.cs",
    "Assets/Scripts/Assembly-CSharp/PercentCloud.cs",
    "Assets/Plugins/Assembly-CSharp-firstpass/GoogleAnalyticsAndroidV3.cs",
    "Assets/Plugins/Assembly-CSharp-firstpass/GoogleAnalyticsiOSV3.cs",
    "Assets/Plugins/Assembly-CSharp-firstpass/GoogleAnalyticsMPV3.cs",
}

BINARY_FAMILY_HINTS = {
    "Facebook.Unity.Android.dll": "social",
    "Facebook.Unity.Gameroom.dll": "social",
    "Facebook.Unity.IOS.dll": "social",
    "Facebook.Unity.Settings.dll": "social",
    "Facebook.Unity.dll": "social",
    "FacebookNamedPipeClient.dll": "social",
    "FacebookStore.dll": "billing",
    "ChannelPurchase.dll": "billing",
    "Purchasing.Common.dll": "billing",
    "Stores.dll": "billing",
    "UnityEngine.Purchasing.dll": "billing",
    "UnityStore.dll": "billing",
    "Unity.Analytics.DataPrivacy.dll": "analytics",
    "Unity.Analytics.StandardEvents.dll": "analytics",
    "Unity.Analytics.Tracker.dll": "analytics",
}

NON_NETWORK_RUNTIME_BINARIES = re.compile(
    r"^(?:DOTween(?:43|46|50|Pro)?|DemiLib|Security|System\.Runtime\.CompilerServices\.Unsafe|"
    r"UnityEngine\.SpatialTrackingModule)\.dll$"
)

ONLINE_SOURCE_PATH = re.compile(
    r"(?:"
    r"GooglePlayGames/|Facebook/Unity/|MoPub|AppsFlyer|GoogleAnalytics|"
    r"UnityEngine/Purchasing/|Percent/Http/|PercentCloud\.cs$|"
    r"/SA/(?:Android/(?:Firebase|GMS|Vending|Social)/|"
    r"iOS/(?:CloudKit|GameKit|StoreKit|Social)/|"
    r"CrossPlatform/(?:Analytics|GameServices|InApp|Social)/|Facebook/)"
    r")",
    re.IGNORECASE,
)

EXAMPLE_RE = re.compile(r"(?:^|[/_.])(?:example|demo|tester?|sample)(?:[/_.]|$)", re.IGNORECASE)
TYPE_DECL_RE = re.compile(r"\b(?:class|struct|interface|enum)\s+([A-Za-z_][A-Za-z0-9_]*)")
TOKEN_RE = re.compile(r"\b[A-Za-z_][A-Za-z0-9_]{3,}\b")
QUALIFIED_TOKEN_RE = re.compile(
    r"(?:global::)?([A-Za-z_][A-Za-z0-9_]*(?:\.[A-Za-z_][A-Za-z0-9_]*)+)"
)
NAMESPACE_RE = re.compile(r"^namespace\s+([A-Za-z_][A-Za-z0-9_.]*)\s*$", re.MULTILINE)
GUID_RE = re.compile(r"\bguid:\s*([0-9a-fA-F]{32})\b")
YAML_DOCUMENT_RE = re.compile(
    r"^--- !u!(\d+) &(-?\d+)\s*\n(.*?)(?=^--- !u!|\Z)", re.MULTILINE | re.DOTALL
)
CALLBACK_TARGET_RE = re.compile(
    r"m_Target:\s*\{fileID:\s*(-?\d+)(?:,[^}]*)?\}\s*\n\s*m_MethodName:\s*([^\n]*)"
)
LOAD_STRING_RE = re.compile(
    r"(?:Resources\.Load(?:All)?(?:<[^>]+>)?|Type\.GetType|AddComponent|GetComponent|"
    r"SendMessage|BroadcastMessage|Invoke|StartCoroutine)\s*\(\s*\"([^\"]+)\""
)
ADD_COMPONENT_GENERIC_RE = re.compile(
    r"AddComponent\s*<\s*(?:global::)?([A-Za-z_][A-Za-z0-9_.]*)\s*>"
)
ADD_COMPONENT_TYPEOF_RE = re.compile(
    r"AddComponent\s*\(\s*typeof\s*\(\s*(?:global::)?([A-Za-z_][A-Za-z0-9_.]*)\s*\)"
)


@dataclass
class Artifact:
    path: str
    kind: str
    family: str
    content: str = ""
    guid: str = ""
    declared_types: tuple[str, ...] = ()
    signals: tuple[str, ...] = ()
    scene_refs: set[str] = field(default_factory=set)
    prefab_refs: set[str] = field(default_factory=set)
    resource_refs: set[str] = field(default_factory=set)
    other_asset_refs: set[str] = field(default_factory=set)
    dormant_asset_refs: set[str] = field(default_factory=set)
    source_refs: set[str] = field(default_factory=set)
    callback_refs: set[str] = field(default_factory=set)
    add_component_refs: set[str] = field(default_factory=set)
    string_load_refs: set[str] = field(default_factory=set)
    dependencies: set[str] = field(default_factory=set)
    reached_by: set[str] = field(default_factory=set)
    classification: str = ""
    reason: str = ""
    action: str = ""
    unresolved: str = ""

    def has_direct_root(self) -> bool:
        return any(
            (
                self.scene_refs,
                self.prefab_refs,
                self.resource_refs,
                self.other_asset_refs,
                self.source_refs,
                self.callback_refs,
                self.add_component_refs,
                self.string_load_refs,
            )
        )


def sha256(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for block in iter(lambda: handle.read(1024 * 1024), b""):
            digest.update(block)
    return digest.hexdigest()


def read_text(path: Path) -> str:
    try:
        return path.read_text(encoding="utf-8")
    except (UnicodeDecodeError, OSError):
        return ""


def family_for(path: str, content: str = "") -> str:
    for family, pattern in FAMILY_RULES:
        if pattern.search(path):
            return family
    probe = content[:65536]
    for family, pattern in FAMILY_RULES:
        if pattern.search(probe):
            return family
    return "platform"


def source_is_candidate(path: str, content: str) -> bool:
    if path.startswith("Assets/Reconstruction/") or path.startswith("Assets/Tests/"):
        return False
    if KNOWN_SDK_PATH.search("/" + path):
        return True
    if path in KNOWN_OFFLINE_SHIMS:
        return True
    if any(pattern.search(content) for pattern in NETWORK_PATTERNS.values()):
        return True
    return any(pattern.search(path) for _, pattern in FAMILY_RULES)


def top_level_type_names(content: str) -> tuple[str, ...]:
    declarations: list[tuple[int, str]] = []
    for line in content.splitlines():
        match = TYPE_DECL_RE.search(line)
        if not match:
            continue
        prefix = line[: len(line) - len(line.lstrip(" \t"))]
        indent = sum(4 if char == "\t" else 1 for char in prefix)
        declarations.append((indent, match.group(1)))
    if not declarations:
        return ()
    minimum_indent = min(indent for indent, _ in declarations)
    return tuple(sorted({name for indent, name in declarations if indent == minimum_indent}))


def source_namespace(content: str) -> str:
    match = NAMESPACE_RE.search(content)
    return match.group(1) if match else ""


def distinctive_type_name(type_name: str) -> bool:
    common = {
        "Builder",
        "Callback",
        "Client",
        "Config",
        "Constants",
        "Context",
        "Data",
        "Event",
        "Factory",
        "Handler",
        "Helper",
        "Internal",
        "Logger",
        "Manager",
        "NativeListener",
        "Object",
        "Options",
        "Parameter",
        "Platform",
        "Request",
        "Response",
        "Result",
        "Settings",
        "Status",
        "Util",
    }
    return type_name not in common and ("_" in type_name or len(type_name) >= 8)


def source_is_production_caller(path: str) -> bool:
    if path.startswith("Assets/Reconstruction/"):
        return True
    if not path.startswith("Assets/Scripts/Assembly-CSharp/"):
        return False
    if KNOWN_SDK_PATH.search("/" + path):
        return False
    if EXAMPLE_RE.search(path):
        return False
    return True


def compact(paths: set[str], limit: int = 4) -> str:
    values = sorted(paths)
    if not values:
        return ""
    if len(values) <= limit:
        return ", ".join(values)
    return ", ".join(values[:limit]) + f", +{len(values) - limit} more"


def markdown_cell(value: str) -> str:
    return value.replace("|", "\\|").replace("\n", " ")


def importer_state(meta_text: str) -> str:
    if "PluginImporter:" not in meta_text:
        return ""
    any_match = re.search(r"Any:\s*\n\s*second:\s*\n\s*enabled:\s*([01])", meta_text)
    editor_match = re.search(r"Editor:\s*Editor\s*\n\s*second:\s*\n\s*enabled:\s*([01])", meta_text)
    pieces = []
    if any_match:
        pieces.append("Any=" + any_match.group(1))
    if editor_match:
        pieces.append("Editor=" + editor_match.group(1))
    return ",".join(pieces)


def locate_monodis() -> Path | None:
    candidates = (
        Path(
            "/Applications/Unity/Hub/Editor/6000.3.18f1/Unity.app/Contents/Resources/"
            "Scripting/MonoBleedingEdge/bin/monodis"
        ),
        Path("/usr/local/bin/monodis"),
        Path("/opt/homebrew/bin/monodis"),
    )
    return next((path for path in candidates if path.is_file()), None)


def managed_assembly_identity(monodis: Path, binary: Path) -> tuple[str, tuple[str, ...]]:
    def invoke(option: str) -> str:
        result = subprocess.run(
            [str(monodis), option, str(binary)],
            check=False,
            stdout=subprocess.PIPE,
            stderr=subprocess.DEVNULL,
            text=True,
            encoding="utf-8",
        )
        return result.stdout if result.returncode == 0 else ""

    assembly_output = invoke("--assembly")
    reference_output = invoke("--assemblyref")
    name_match = re.search(r"^Name:\s*(\S.*?)\s*$", assembly_output, re.MULTILINE)
    references = tuple(sorted(set(re.findall(r"^\s*Name=(\S.*?)\s*$", reference_output, re.MULTILINE))))
    return (name_match.group(1) if name_match else binary.stem, references)


def has_online_capability(artifact: Artifact) -> bool:
    if artifact.kind == "plugin binary":
        return artifact.family in {"ads", "analytics", "attribution", "billing", "cloud", "network", "social"}
    active_signal = any(
        signal in {"unity-www", "dotnet-network", "socket"} for signal in artifact.signals
    )
    return active_signal or bool(ONLINE_SOURCE_PATH.search(artifact.path))


def build_audit(repo: Path) -> tuple[list[Artifact], dict[str, object]]:
    assets = repo / "Assets"
    source_paths = sorted(assets.rglob("*.cs"))
    source_text = {p.relative_to(repo).as_posix(): read_text(p) for p in source_paths}
    all_declared_types: dict[str, set[str]] = defaultdict(set)
    for rel, content in source_text.items():
        for type_name in top_level_type_names(content):
            all_declared_types[type_name].add(rel)

    artifacts: dict[str, Artifact] = {}
    for rel, content in source_text.items():
        if not source_is_candidate(rel, content):
            continue
        meta_path = repo / (rel + ".meta")
        meta_text = read_text(meta_path)
        guid_match = GUID_RE.search(meta_text)
        signals = tuple(name for name, pattern in NETWORK_PATTERNS.items() if pattern.search(content))
        declared = top_level_type_names(content)
        artifacts[rel] = Artifact(
            path=rel,
            kind="source",
            family=family_for(rel, content),
            content=content,
            guid=guid_match.group(1).lower() if guid_match else "",
            declared_types=declared,
            signals=signals,
        )

    binary_paths = sorted(
        p for p in (assets / "Plugins").rglob("*") if p.is_file() and p.suffix in BINARY_SUFFIXES
    )
    for binary in binary_paths:
        rel = binary.relative_to(repo).as_posix()
        meta_text = read_text(Path(str(binary) + ".meta"))
        guid_match = GUID_RE.search(meta_text)
        family = BINARY_FAMILY_HINTS.get(binary.name, family_for(rel))
        artifacts[rel] = Artifact(
            path=rel,
            kind="plugin binary",
            family=family,
            guid=guid_match.group(1).lower() if guid_match else "",
            signals=("binary-import:" + importer_state(meta_text),),
        )

    monodis = locate_monodis()
    assembly_name_to_path: dict[str, str] = {}
    binary_references: dict[str, tuple[str, ...]] = {}
    if monodis:
        for binary in binary_paths:
            rel = binary.relative_to(repo).as_posix()
            assembly_name, references = managed_assembly_identity(monodis, binary)
            assembly_name_to_path[assembly_name] = rel
            binary_references[rel] = references
        for rel, references in binary_references.items():
            for reference in references:
                dependency = assembly_name_to_path.get(reference)
                if dependency and dependency != rel:
                    artifacts[rel].dependencies.add(dependency)

    candidate_paths = set(artifacts)
    type_to_candidates: dict[str, set[str]] = defaultdict(set)
    distinctive_type_to_candidates: dict[str, set[str]] = defaultdict(set)
    qualified_type_to_candidates: dict[str, set[str]] = defaultdict(set)
    namespace_by_source = {rel: source_namespace(content) for rel, content in source_text.items()}
    for artifact in artifacts.values():
        if artifact.kind == "source":
            for type_name in artifact.declared_types:
                if len(type_name) >= 4 and len(all_declared_types[type_name]) == 1:
                    type_to_candidates[type_name].add(artifact.path)
                    if distinctive_type_name(type_name):
                        distinctive_type_to_candidates[type_name].add(artifact.path)
                namespace = namespace_by_source.get(artifact.path, "")
                if namespace:
                    qualified_type_to_candidates[namespace + "." + type_name].add(artifact.path)

    build_settings = read_text(repo / "ProjectSettings/EditorBuildSettings.asset")
    enabled_scenes = set(
        re.findall(r"- enabled:\s*1\s*\n\s*path:\s*([^\n]+)", build_settings)
    )

    serialized_paths = sorted(
        p for p in assets.rglob("*") if p.is_file() and p.suffix in SERIALIZED_SUFFIXES
    )
    serialized_text: dict[str, str] = {}
    guid_to_assets: dict[str, set[str]] = defaultdict(set)
    for item in serialized_paths:
        text = read_text(item)
        if not text:
            continue
        rel = item.relative_to(repo).as_posix()
        serialized_text[rel] = text
        for guid in set(GUID_RE.findall(text)):
            guid_to_assets[guid.lower()].add(rel)

    asset_guid_to_path: dict[str, str] = {}
    for meta in sorted(assets.rglob("*.meta")):
        asset_path = Path(str(meta)[:-5])
        if not asset_path.is_file():
            continue
        match = GUID_RE.search(read_text(meta))
        if match:
            asset_guid_to_path[match.group(1).lower()] = asset_path.relative_to(repo).as_posix()

    serialized_set = set(serialized_text)
    asset_dependencies: dict[str, set[str]] = defaultdict(set)
    for rel, text in serialized_text.items():
        for guid in set(GUID_RE.findall(text)):
            target = asset_guid_to_path.get(guid.lower())
            if target in serialized_set and target != rel:
                asset_dependencies[rel].add(target)
    reachable_assets = set(enabled_scenes)
    reachable_assets.update(rel for rel in serialized_set if rel.startswith("Assets/Resources/"))
    asset_queue: deque[str] = deque(sorted(reachable_assets))
    while asset_queue:
        caller = asset_queue.popleft()
        for dependency in asset_dependencies.get(caller, set()):
            if dependency not in reachable_assets:
                reachable_assets.add(dependency)
                asset_queue.append(dependency)

    source_guid_to_path: dict[str, str] = {}
    for rel in source_text:
        match = GUID_RE.search(read_text(repo / (rel + ".meta")))
        if match:
            source_guid_to_path[match.group(1).lower()] = rel
    reachable_source_paths: set[str] = set()
    for rel in reachable_assets:
        for guid in set(GUID_RE.findall(serialized_text.get(rel, ""))):
            source_path = source_guid_to_path.get(guid.lower())
            if source_path:
                reachable_source_paths.add(source_path)
    reachable_source_paths.update(
        rel for rel, content in source_text.items() if "RuntimeInitializeOnLoadMethod" in content
    )

    candidate_guid_to_path = {
        artifact.guid: artifact.path
        for artifact in artifacts.values()
        if artifact.kind == "source" and artifact.guid
    }
    callback_by_candidate: dict[str, set[str]] = defaultdict(set)
    dormant_callback_by_candidate: dict[str, set[str]] = defaultdict(set)
    for rel, text in serialized_text.items():
        target_by_file_id: dict[str, str] = {}
        for unity_class, file_id, document in YAML_DOCUMENT_RE.findall(text):
            if unity_class != "114":
                continue
            script_match = re.search(r"m_Script:\s*\{[^}]*guid:\s*([0-9a-fA-F]{32})", document)
            if not script_match:
                continue
            candidate = candidate_guid_to_path.get(script_match.group(1).lower())
            if candidate:
                target_by_file_id[file_id] = candidate
        for file_id, method_name in CALLBACK_TARGET_RE.findall(text):
            candidate = target_by_file_id.get(file_id)
            if not candidate:
                continue
            record = rel + "#" + method_name.strip()
            if rel in reachable_assets:
                callback_by_candidate[candidate].add(record)
            else:
                dormant_callback_by_candidate[candidate].add(record)

    for artifact in artifacts.values():
        if artifact.kind != "source" or not artifact.guid:
            continue
        for ref in guid_to_assets.get(artifact.guid, set()):
            if ref not in reachable_assets:
                artifact.dormant_asset_refs.add(ref)
            elif ref in enabled_scenes:
                artifact.scene_refs.add(ref)
            elif ref.endswith(".prefab"):
                artifact.prefab_refs.add(ref)
            elif ref.startswith("Assets/Resources/"):
                artifact.resource_refs.add(ref)
            else:
                artifact.other_asset_refs.add(ref)
        artifact.callback_refs.update(callback_by_candidate.get(artifact.path, set()))
        artifact.dormant_asset_refs.update(dormant_callback_by_candidate.get(artifact.path, set()))

    tokens_by_source = {rel: set(TOKEN_RE.findall(content)) for rel, content in source_text.items()}
    qualified_by_source = {
        rel: set(QUALIFIED_TOKEN_RE.findall(content)) for rel, content in source_text.items()
    }
    for caller, tokens in tokens_by_source.items():
        targets: set[str] = set()
        for type_name in tokens.intersection(distinctive_type_to_candidates):
            targets.update(distinctive_type_to_candidates[type_name])
        for qualified_name in qualified_by_source[caller].intersection(qualified_type_to_candidates):
            targets.update(qualified_type_to_candidates[qualified_name])
        namespace = namespace_by_source.get(caller, "")
        if namespace:
            for type_name in tokens:
                targets.update(qualified_type_to_candidates.get(namespace + "." + type_name, set()))
        for callee in targets:
            if caller == callee:
                continue
            if caller in candidate_paths:
                artifacts[caller].dependencies.add(callee)
            elif source_is_production_caller(caller) and caller in reachable_source_paths:
                artifacts[callee].source_refs.add(caller)

    for caller, content in source_text.items():
        for literal in LOAD_STRING_RE.findall(content):
            literal_type = literal.rsplit(".", 1)[-1]
            for callee in type_to_candidates.get(literal_type, set()):
                artifacts[callee].string_load_refs.add(caller + "#" + literal)
        component_types = set(ADD_COMPONENT_GENERIC_RE.findall(content))
        component_types.update(ADD_COMPONENT_TYPEOF_RE.findall(content))
        for qualified_name in component_types:
            type_name = qualified_name.rsplit(".", 1)[-1]
            callees = set(qualified_type_to_candidates.get(qualified_name, set()))
            callees.update(type_to_candidates.get(type_name, set()))
            for callee in callees:
                if caller != callee:
                    artifacts[callee].add_component_refs.add(caller)

    # Attribute binary use to source namespaces and known runtime packages.
    production_source = "\n".join(
        content
        for path, content in source_text.items()
        if source_is_production_caller(path) and path in reachable_source_paths
    )
    for artifact in artifacts.values():
        if artifact.kind != "plugin binary":
            continue
        name = Path(artifact.path).name
        stem = name.removesuffix(".dll")
        if NON_NETWORK_RUNTIME_BINARIES.match(name):
            probes = {
                "DOTween": ("DG.Tweening", "DOTween"),
                "DemiLib": ("DemiLib",),
                "Security": ("__no_direct_security_probe__",),
                "System.Runtime.CompilerServices.Unsafe": ("System.Runtime.CompilerServices.Unsafe",),
                "UnityEngine.SpatialTrackingModule": ("UnityEngine.SpatialTracking",),
            }
            root = re.sub(r"(?:43|46|50|Pro)$", "", stem)
            if any(token in production_source for token in probes.get(root, (stem,))):
                artifact.source_refs.add("compiled source namespace:" + root)
        elif artifact.family == "social" and "Facebook.Unity" in production_source:
            artifact.source_refs.add("compiled source namespace:Facebook.Unity")
        elif name == "UnityEngine.Purchasing.dll" and "UnityEngine.Purchasing" in production_source:
            artifact.source_refs.add("compiled source namespace:UnityEngine.Purchasing")
        elif artifact.family == "analytics" and "UnityEngine.Analytics" in production_source:
            artifact.source_refs.add("compiled source namespace:UnityEngine.Analytics")

    # Roots are proof from non-candidate sources or serialized content. Offline
    # shims are retained roots even when only their serialized API is needed.
    queue: deque[str] = deque()
    reachable: set[str] = set()
    for artifact in artifacts.values():
        if artifact.has_direct_root():
            reachable.add(artifact.path)
            queue.append(artifact.path)
    while queue:
        caller = queue.popleft()
        for callee in artifacts[caller].dependencies:
            artifacts[callee].reached_by.add(caller)
            if callee not in reachable:
                reachable.add(callee)
                queue.append(callee)

    for artifact in artifacts.values():
        is_example = bool(EXAMPLE_RE.search(artifact.path))
        online_capable = has_online_capability(artifact)
        if artifact.path in KNOWN_OFFLINE_SHIMS or "GoodPirate113.Reconstruction.Offline" in artifact.content:
            artifact.classification = "offline shim"
            artifact.reason = "Retained public API now routes to a local no-op, unavailable result, or explicit local reward."
            artifact.action = "Retain path, GUID, and signature. Test the real callback path before removing dependencies."
        elif "/Editor/" in artifact.path or artifact.path.startswith("Assets/Editor/"):
            artifact.classification = "editor-only"
            artifact.reason = "Editor path is not a player-runtime root."
            artifact.action = "Keep only if reconstruction tooling needs it, otherwise remove with its meta after the GUID audit."
        elif is_example and artifact.path not in reachable:
            artifact.classification = "example/test"
            artifact.reason = "Example or demo naming and no proven production root."
            artifact.action = "Removal candidate after recording its GUID and confirming no dynamic string construction."
        elif artifact.path in reachable:
            artifact.classification = "live runtime support"
            roots = set().union(
                artifact.scene_refs,
                artifact.prefab_refs,
                artifact.resource_refs,
                artifact.source_refs,
                artifact.callback_refs,
                artifact.add_component_refs,
                artifact.string_load_refs,
                artifact.reached_by,
            )
            artifact.reason = "Statically reachable from " + compact(roots) + "."
            if online_capable:
                artifact.action = "Replace with a narrow offline facade, then rerun this audit before deleting the payload."
            else:
                artifact.action = "Retain until the dependent gameplay closure and serialized references are validated."
        else:
            artifact.classification = "removal candidate"
            artifact.reason = "No reachable serialized, callback, production-source, Resources/string, AddComponent, or transitive root was proven."
            artifact.action = "Remove only as a reviewed path plus meta change, then compile and run structural callback tests."

        if artifact.classification == "offline shim" and artifact.signals:
            unsafe = [signal for signal in artifact.signals if signal != "native-bridge"]
            if unsafe:
                if unsafe == ["url"]:
                    artifact.unresolved = "Shim retains an embedded URL or domain that must be removed before artifact acceptance."
                else:
                    artifact.unresolved = "Shim still contains network-capable signals: " + ", ".join(unsafe)
        elif artifact.classification == "live runtime support" and online_capable:
            artifact.unresolved = "Legacy online family remains in the statically reachable closure."

    input_paths = sorted(
        set(source_paths)
        | set(binary_paths)
        | {Path(str(p) + ".meta") for p in source_paths + binary_paths if Path(str(p) + ".meta").is_file()}
        | set(serialized_paths)
        | {repo / "ProjectSettings/EditorBuildSettings.asset"}
        | {Path(__file__).resolve()}
    )
    fingerprint = hashlib.sha256()
    for path in input_paths:
        if not path.is_file():
            continue
        rel = path.relative_to(repo).as_posix()
        fingerprint.update(rel.encode("utf-8"))
        fingerprint.update(b"\0")
        fingerprint.update(sha256(path).encode("ascii"))
        fingerprint.update(b"\n")

    metadata: dict[str, object] = {
        "enabled_scenes": sorted(enabled_scenes),
        "input_fingerprint": fingerprint.hexdigest(),
        "source_count": len(source_paths),
        "serialized_count": len(serialized_paths),
        "reachable_asset_count": len(reachable_assets),
        "reachable_source_count": len(reachable_source_paths),
        "monodis": str(monodis) if monodis else "unavailable",
        "monodis_sha256": sha256(monodis) if monodis else "unavailable",
    }
    return sorted(artifacts.values(), key=lambda item: item.path), metadata


def render_report(artifacts: list[Artifact], metadata: dict[str, object]) -> str:
    classifications = Counter(item.classification for item in artifacts)
    kinds = Counter(item.kind for item in artifacts)
    families = Counter(item.family for item in artifacts)
    unresolved = [item for item in artifacts if item.unresolved]
    zero_ref = [item for item in artifacts if item.classification in {"removal candidate", "example/test"}]
    by_path = {item.path: item for item in artifacts}
    unresolved_paths = {item.path for item in unresolved}
    closure_roots: list[tuple[Artifact, int]] = []
    for root in artifacts:
        if not root.has_direct_root():
            continue
        seen: set[str] = set()
        queue = deque([root.path])
        while queue:
            path = queue.popleft()
            if path in seen:
                continue
            seen.add(path)
            queue.extend(by_path[path].dependencies - seen)
        unresolved_count = len(seen.intersection(unresolved_paths))
        if unresolved_count:
            closure_roots.append((root, unresolved_count))
    closure_roots.sort(key=lambda pair: (-pair[1], pair[0].path))

    lines = [
        "# Good Pirate 1.13 legacy SDK reachability",
        "",
        f"Schema: `{REPORT_SCHEMA}`",
        "",
        f"Input fingerprint: `{metadata['input_fingerprint']}`",
        "",
        "This is a conservative static audit. A live classification proves a static path. A removal candidate means no path was proven by the indexed mechanisms, not that runtime reflection is impossible. Every removal still requires its `.meta` GUID to be allowlisted, a clean Unity compile, and callback and structural tests.",
        "",
        "## Scope and evidence",
        "",
        f"- Indexed {metadata['source_count']} recovered C# files and {metadata['serialized_count']} serialized Unity assets.",
        f"- The enabled-scene and Resources closure contains {metadata['reachable_asset_count']} serialized assets and {metadata['reachable_source_count']} attached or runtime-initialized source files.",
        f"- Audited {len(artifacts)} candidates: {kinds.get('source', 0)} source files and {kinds.get('plugin binary', 0)} plugin binaries.",
        f"- Managed plugin assembly references were read with `{metadata['monodis']}` SHA-256 `{metadata['monodis_sha256']}`.",
        "- Static roots include enabled scenes, recursively referenced prefabs and assets, all Resources content, persistent UnityEvent target and method records, calls from attached production source, string-based loads and messages, and runtime `AddComponent` calls.",
        "- Candidate dependencies are followed transitively from proven roots. Native reflection, runtime-generated type names, JNI internals, and asset-bundle content cannot be disproved statically.",
        "",
        "Enabled scenes: " + ", ".join(f"`{scene}`" for scene in metadata["enabled_scenes"]),
        "",
        "## Classification summary",
        "",
        "| Classification | Count |",
        "|---|---:|",
    ]
    for key in ("live runtime support", "offline shim", "editor-only", "example/test", "removal candidate"):
        lines.append(f"| {key} | {classifications.get(key, 0)} |")
    lines.extend(
        [
            "",
            "Family counts: " + ", ".join(f"{key} {value}" for key, value in sorted(families.items())),
            "",
            f"Zero-root cleanup set: {len(zero_ref)} artifacts. Unresolved reachable online paths: {len(unresolved)} artifacts.",
            "",
            "## Proven closure roots",
            "",
            f"These {len(closure_roots)} artifacts have a static path into the unresolved online closure through production data or code. Descendant counts overlap and do not imply exclusive ownership. Removing only transitive support files will not close the corresponding SDK family.",
            "",
            "| Path | Unresolved descendants | Classification | Direct proof | Required first action |",
            "|---|---:|---|---|---|",
        ]
    )
    for item, unresolved_count in closure_roots:
        proof = set().union(
            item.scene_refs,
            item.prefab_refs,
            item.resource_refs,
            item.other_asset_refs,
            item.source_refs,
            item.callback_refs,
            item.add_component_refs,
            item.string_load_refs,
        )
        action = item.action
        if item.classification == "offline shim":
            action = "Retain the path, GUID, and public API while replacing its remaining online-typed dependencies with narrow compatibility types."
        elif source_is_production_caller(item.path):
            action = "Retain the gameplay class and replace only its online dependency edges with local services or compatibility types."
        elif not has_online_capability(item):
            action = "Remove or narrow this reachable root before pruning its online support descendants."
        lines.append(
            f"| `{markdown_cell(item.path)}` | {unresolved_count} | {item.classification} | {markdown_cell(compact(proof))} | {markdown_cell(action)} |"
        )
    lines.extend(
        [
            "",
            "## Unresolved reachable online paths",
            "",
        ]
    )
    if unresolved:
        lines.extend(["| Path | Family | Classification | Proof | Risk |", "|---|---|---|---|---|"])
        for item in unresolved:
            proof = compact(
                set().union(
                    item.scene_refs,
                    item.prefab_refs,
                    item.resource_refs,
                    item.source_refs,
                    item.callback_refs,
                    item.add_component_refs,
                    item.string_load_refs,
                    item.reached_by,
                )
            )
            lines.append(
                f"| `{markdown_cell(item.path)}` | {item.family} | {item.classification} | "
                f"{markdown_cell(proof)} | {markdown_cell(item.unresolved)} |"
            )
    else:
        lines.append("No unresolved path was found by the indexed static mechanisms.")

    lines.extend(
        [
            "",
            "## Action order",
            "",
            "1. Preserve every offline shim path, GUID, public method, and serialized callback signature.",
            "2. Replace unresolved reachable online families with narrow local facades before removing their support graphs.",
            "3. Remove example/test and removal-candidate rows in reviewed family-sized changes. Record every removed GUID in the migration allowlist.",
            "4. After each removal, run a clean Unity compile, scene and prefab missing-script audit, all persistent callback tests, and this script again.",
            "5. Treat plugin binaries as unresolved until the source namespace dependency and platform importer state are both eliminated.",
            "",
            "## Artifact ledger",
            "",
            "Counts in the reference columns are exact. Samples are limited to four paths for readability.",
            "",
            "| Path | GUID | Kind | Family | Classification | Network signals | Scenes | Prefabs | Resources | Other reachable assets | Dormant assets | Source callers | Callbacks | AddComponent | String loads | Reached by | Action |",
            "|---|---|---|---|---|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---|",
        ]
    )
    for item in artifacts:
        lines.append(
            "| `{}` | `{}` | {} | {} | {} | {} | {} | {} | {} | {} | {} | {} | {} | {} | {} | {} | {} |".format(
                markdown_cell(item.path),
                item.guid,
                item.kind,
                item.family,
                item.classification,
                markdown_cell(", ".join(item.signals)),
                len(item.scene_refs),
                len(item.prefab_refs),
                len(item.resource_refs),
                len(item.other_asset_refs),
                len(item.dormant_asset_refs),
                len(item.source_refs),
                len(item.callback_refs),
                len(item.add_component_refs),
                len(item.string_load_refs),
                len(item.reached_by),
                markdown_cell(item.action),
            )
        )

    lines.extend(
        [
            "",
            "## Proof samples",
            "",
            "| Path | Reason | Serialized and source proof sample |",
            "|---|---|---|",
        ]
    )
    for item in artifacts:
        proof = set().union(
            item.scene_refs,
            item.prefab_refs,
            item.resource_refs,
            item.other_asset_refs,
            item.dormant_asset_refs,
            item.source_refs,
            item.callback_refs,
            item.add_component_refs,
            item.string_load_refs,
            item.reached_by,
        )
        if proof or item.classification == "offline shim":
            lines.append(
                f"| `{markdown_cell(item.path)}` | {markdown_cell(item.reason)} | {markdown_cell(compact(proof))} |"
            )
    lines.append("")
    return "\n".join(lines)


def main() -> int:
    parser = argparse.ArgumentParser()
    parser.add_argument(
        "--repo",
        type=Path,
        default=Path(__file__).resolve().parents[2],
        help="Unity reconstruction repository root",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=Path("docs/evidence/legacy-sdk-reachability.md"),
        help="Report path relative to the repository root",
    )
    args = parser.parse_args()
    repo = args.repo.resolve()
    output = args.output if args.output.is_absolute() else repo / args.output
    artifacts, metadata = build_audit(repo)
    report = render_report(artifacts, metadata)
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text(report, encoding="utf-8", newline="\n")
    print(f"wrote {output}")
    print(f"artifacts={len(artifacts)} unresolved={sum(bool(item.unresolved) for item in artifacts)}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
