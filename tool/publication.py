#!/usr/bin/env python3
"""Package and verify the exact locally accepted Good Pirate preview."""
import argparse
import hashlib
import json
from pathlib import Path, PurePosixPath
import re
import shutil
import stat
import subprocess
import sys
import xml.etree.ElementTree as ET
import zipfile

ROOT = Path(__file__).resolve().parent.parent
VERSION = "v0.1.0-preview.1"
TAG = "gp113-" + VERSION
ARCHIVE = "good-pirate-webgl-" + VERSION + ".zip"
REQUIRED = ["fresh_tutorial", "steering", "docking", "combat_rewards", "upgrade",
            "death_recovery", "modal_closure", "all_16_products", "clipboard_copy",
            "clipboard_import", "import_cancel", "corrupt_rejection",
            "backup_recovery", "reload_persistence", "origin_transfer"]

def digest(p):
    return hashlib.sha256(p.read_bytes()).hexdigest()

def git(*args):
    return subprocess.check_output(["git", "-C", str(ROOT), *args], text=True).strip()

def write_json(p, data):
    p.write_text(json.dumps(data, indent=2, sort_keys=True) + "\n")

def clean_source():
    if git("status", "--porcelain", "--untracked-files=normal"):
        raise ValueError("Publication source has uncommitted changes")

def build_commit():
    commit = (ROOT / "Builds/WebGL/publication/source-commit.txt").read_text().strip()
    if not re.fullmatch(r"[0-9a-f]{40}", commit): raise ValueError("Invalid build source identity")
    for tree in ["Assets", "Packages", "ProjectSettings"]:
        if git("rev-parse", commit + ":" + tree) != git("rev-parse", "HEAD:" + tree):
            raise ValueError("Runtime source changed after building: " + tree)
    return commit

def audit():
    forbidden = {".apk", ".aab", ".jks", ".keystore", ".so", ".dex"}
    issues = []
    tracked = git("ls-files", "--cached", "--others", "--exclude-standard").splitlines()
    for name in tracked:
        p = ROOT / name
        if not p.is_file(): continue
        if p.is_symlink() or p.suffix.lower() in forbidden or p.stat().st_size >= 100 * 1024**2:
            issues.append(name + ": excluded payload or oversized file")
        if p.suffix == ".dll" and name != "Assets/Plugins/Demigiant/DOTween/DOTween.dll":
            issues.append(name + ": unreviewed plugin binary")
        if p.suffix in {".cs", ".json", ".md", ".py", ".sh", ".tsv", ".yml", ".asset"}:
            text = p.read_text(errors="replace")
            if re.search(r"/Users/[A-Za-z0-9_.-]+/", text):
                issues.append(name + ": machine-specific path")
            if re.search(r"(?:ghp_[A-Za-z0-9]{30,}|github_pat_[A-Za-z0-9_]{30,}|-----BEGIN (?:RSA |OPENSSH |EC )?PRIVATE KEY-----)", text):
                issues.append(name + ": credential material")
    for name in ["Assets/Resources/UM_Settings.asset", "Assets/Resources/AN_Settings.asset", "Assets/Resources/ISN_Settings.asset", "Assets/Resources/FacebookSettings.asset"]:
        if (ROOT / name).exists(): issues.append(name + ": retired SDK configuration")
    if issues: raise ValueError("\n".join(issues))
    print(f"PASS: publication boundary, {len(tracked)} source files")

def candidate():
    clean_source()
    build = ROOT / "Builds/WebGL/publication"
    commit = build_commit()
    if (build / "source-commit.txt").read_text().strip() != commit:
        raise ValueError("Build source differs from the current commit")
    if "unity-development-button" in (build / "index.html").read_text():
        raise ValueError("Development player cannot be published")
    if not list((build / "Build").glob("*.unityweb")):
        raise ValueError("Expected compressed build with decompression fallback")
    site = ROOT / "Builds/PublicTest/good-pirate"
    if site.exists(): shutil.rmtree(site)
    site.mkdir(parents=True)
    shutil.copytree(build, site / "play")
    shutil.copyfile(ROOT / "site/index.html", site / "index.html")
    (site / ".nojekyll").touch()
    metadata = {"version": VERSION, "tag": TAG, "source_commit": commit, "publication_commit": git("rev-parse", "HEAD"),
                "unity_version": "6000.3.18f1", "status": "candidate"}
    metadata["files"] = {p.relative_to(site).as_posix(): digest(p) for p in sorted(site.rglob("*")) if p.is_file()}
    write_json(site / "publication.json", metadata)
    print(site)
    return site

def acceptance(path, commit):
    data = json.loads(path.read_text())
    return validate_acceptance(data, commit)

def validate_acceptance(data, commit):
    if data.get("source_commit") != commit: raise ValueError("Acceptance belongs to a different source commit")
    if data.get("level") == "smoke":
        policy = data.get("test_policy", {})
        if policy.get("max_test_seconds") != 60 or policy.get("requested_by_user") is not True:
            raise ValueError("A smoke preview requires the explicit shortened test policy")
        if data.get("browsers", {}).get("chromium", {}).get("status") != "passed":
            raise ValueError("Chromium smoke evidence is missing")
        for name in ["startup", "steering", "tutorial_combat", "settings", "copy_save"]:
            item = data.get("smoke_checks", {}).get(name, {})
            if item.get("status") != "passed" or not item.get("evidence"):
                raise ValueError("Missing smoke evidence: " + name)
        if not data.get("limitations"): raise ValueError("A smoke preview must disclose its untested coverage")
        if data.get("uncaught_errors") != 0 or data.get("unexpected_application_requests") != 0:
            raise ValueError("Captured smoke console or network checks failed")
        return data
    if data.get("gameplay_interaction_seconds", 0) < 1800: raise ValueError("30 minutes of actual interaction are required")
    browsers = data.get("browsers", {})
    for name in ["chromium", "firefox", "mobile_physical"]:
        entry = browsers.get(name, {})
        if entry.get("status") != "passed" or not entry.get("evidence"):
            raise ValueError(f"Missing accepted browser evidence: {name}")
    for name in REQUIRED:
        entry = data.get("scenarios", {}).get(name, {})
        if entry.get("status") != "passed" or not entry.get("evidence"):
            raise ValueError(f"Missing accepted scenario evidence: {name}")
    if data.get("uncaught_errors") != 0 or data.get("unexpected_application_requests") != 0:
        raise ValueError("Console and network acceptance failed")
    return data

def package(path):
    audit()
    clean_source()
    commit = build_commit()
    accepted = acceptance(path, commit)
    tests = ET.parse(ROOT / "Logs/editmode-results.xml").getroot()
    if tests.get("result") != "Passed" or int(tests.get("failed", "1")) != 0:
        raise ValueError("Edit Mode acceptance failed")
    site = ROOT / "Builds/PublicTest/good-pirate"
    candidate_metadata = json.loads((site / "publication.json").read_text())
    if candidate_metadata["source_commit"] != commit:
        raise ValueError("Tested candidate differs from the current commit")
    for name, expected_hash in candidate_metadata["files"].items():
        if digest(site / name) != expected_hash: raise ValueError("Candidate changed after assembly: " + name)
    output = ROOT / "Builds/Release"
    output.mkdir(parents=True, exist_ok=True)
    manifest = {"version": VERSION, "tag": TAG, "source_commit": commit,
                "publication_commit": git("rev-parse", "HEAD"),
                "unity_version": "6000.3.18f1", "status": "smoke-tested-preview" if accepted.get("level") == "smoke" else "accepted-preview",
                "editmode": {"passed": int(tests.get("passed")), "failed": 0},
                "acceptance": accepted, "files": {}}
    for p in sorted(site.rglob("*")):
        if p.is_file() and p.name != "publication.json":
            manifest["files"][p.relative_to(site).as_posix()] = digest(p)
    write_json(site / "publication.json", manifest)
    write_json(output / "publication.json", manifest)
    with zipfile.ZipFile(output / ARCHIVE, "w", zipfile.ZIP_DEFLATED) as z:
        for p in sorted(site.rglob("*")):
            if p.is_file(): z.write(p, p.relative_to(site).as_posix())
    (output / "SHA256SUMS").write_text("\n".join(f"{digest(output / name)}  {name}" for name in [ARCHIVE, "publication.json"]) + "\n")
    print(output / ARCHIVE)

def verify(bundle, manifest_path, expected_commit, expected_tag, output):
    manifest = json.loads(manifest_path.read_text())
    if manifest.get("source_commit") != expected_commit or manifest.get("tag") != expected_tag or manifest.get("status") not in {"accepted-preview", "smoke-tested-preview"}:
        raise ValueError("Release provenance mismatch")
    if (manifest["status"] == "smoke-tested-preview") != (manifest.get("acceptance", {}).get("level") == "smoke"):
        raise ValueError("Release validation label mismatch")
    validate_acceptance(manifest.get("acceptance", {}), expected_commit)
    if output.exists(): raise ValueError("Output directory already exists")
    output.mkdir(parents=True)
    with zipfile.ZipFile(bundle) as z:
        names = set()
        for info in z.infolist():
            path = PurePosixPath(info.filename)
            if path.is_absolute() or ".." in path.parts or "\\" in info.filename or stat.S_ISLNK(info.external_attr >> 16) or info.filename in names:
                raise ValueError("Unsafe release archive member")
            names.add(info.filename)
        expected = set(manifest["files"]) | {"publication.json"}
        if names != expected or sum(i.file_size for i in z.infolist()) > 1024**3:
            raise ValueError("Release archive inventory mismatch")
        for name, expected_hash in manifest["files"].items():
            if hashlib.sha256(z.read(name)).hexdigest() != expected_hash: raise ValueError("Release file checksum mismatch: " + name)
        if json.loads(z.read("publication.json")) != manifest: raise ValueError("Embedded manifest mismatch")
        z.extractall(output)
    print("PASS: accepted release checksums, provenance and archive inventory")

def main():
    p = argparse.ArgumentParser()
    sub = p.add_subparsers(dest="command", required=True)
    sub.add_parser("audit")
    sub.add_parser("candidate")
    package_parser = sub.add_parser("package")
    package_parser.add_argument("--acceptance", type=Path, required=True)
    verify_parser = sub.add_parser("verify")
    verify_parser.add_argument("--bundle", type=Path, required=True)
    verify_parser.add_argument("--manifest", type=Path, required=True)
    verify_parser.add_argument("--commit", required=True)
    verify_parser.add_argument("--tag", required=True)
    verify_parser.add_argument("--output", type=Path, required=True)
    args = p.parse_args()
    if args.command == "audit": audit()
    elif args.command == "candidate": candidate()
    elif args.command == "package": package(args.acceptance)
    else: verify(args.bundle, args.manifest, args.commit, args.tag, args.output)

if __name__ == "__main__":
    try: main()
    except (ValueError, OSError, KeyError) as e:
        print("Publication refused: " + str(e), file=sys.stderr)
        sys.exit(1)
