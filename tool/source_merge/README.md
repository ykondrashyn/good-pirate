# Source merge gate

`plan_merge.py` maps ILSpy donor files to AssetRipper-owned Unity script files
by their exact set of fully qualified top-level types. It preserves the Unity
file path and `.meta` file. Ambiguous mappings are never applied automatically.

Dry run:

```sh
python3 tool/source_merge/plan_merge.py \
  --donor ../good_pirate_1_13_evidence/decompiled/Assembly-CSharp \
  --destination /path/to/assetripper/ExportedProject/Assets/Scripts \
  --report /tmp/gp113-source-merge.tsv
```

After reviewing every `review` row, repeat with `--apply` against a disposable
copy. Do not run apply directly against the only AssetRipper export.

