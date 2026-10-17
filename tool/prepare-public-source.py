#!/usr/bin/env python3
"""Remove recovered SDK payloads from a disposable publication snapshot.

Original evidence and the development checkout are never modified.
"""
import hashlib
from pathlib import Path
import shutil
import sys

root = Path(__file__).resolve().parent.parent
archive = Path(sys.argv[1]).resolve()
if root.name != "good-pirate-publication" or archive.exists():
    raise SystemExit("Expected a fresh publication checkout and archive directory")
archive.mkdir(parents=True)
keep = {"GoogleAnalyticsV3.cs", "GoogleAnalyticsV3.cs.meta",
        "AppsFlyerTrackerCallbacks.cs", "AppsFlyerTrackerCallbacks.cs.meta"}
selected = []
for p in (root / "Assets/Plugins").rglob("*"):
    if not p.is_file():
        continue
    rel = p.relative_to(root).as_posix()
    if p.name.endswith((".dll", ".dll.meta")):
        selected.append(p)
    elif "Assembly-CSharp-firstpass/" in rel:
        if "/UnityStandardAssets/Water/" not in rel and p.name not in keep:
            selected.append(p)
for part in ["Assets/Scripts/Assembly-CSharp/Facebook",
             "Assets/Scripts/Assembly-CSharp/GooglePlayGames"]:
    selected.extend(p for p in (root / part).rglob("*") if p.is_file())
for name in ["UM_Settings", "AN_Settings", "ISN_Settings", "FacebookSettings"]:
    selected.extend((root / "Assets/Resources").glob(name + ".asset*"))
ledger = ["path\tsha256\treason"]
for p in sorted(set(selected)):
    rel = p.relative_to(root)
    ledger.append(f"{rel.as_posix()}\t{hashlib.sha256(p.read_bytes()).hexdigest()}\tobsolete SDK or replaced dependency")
    destination = archive / rel
    destination.parent.mkdir(parents=True, exist_ok=True)
    shutil.move(str(p), destination)
(root / "docs/publication").mkdir(parents=True, exist_ok=True)
(root / "docs/publication/removed-payloads.tsv").write_text("\n".join(ledger) + "\n")
print(f"Archived {len(selected)} payload files outside the publication checkout")
