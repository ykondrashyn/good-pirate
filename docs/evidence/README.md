# Version 1.13 recovery evidence

Files in this directory are byte-stable indexes generated from the exact Good
Pirate 1.13 APK. They are evidence manifests, not recovered original source.

- `package-summary.tsv` records package, Unity, ABI, and scripting-backend facts.
- `apk-entry-manifest.tsv` hashes every APK member.
- `native-library-census.tsv` records the ARM native payload.
- `managed-assembly-census.tsv` records assembly structure and hashes.
- `normalized-cil-manifest.tsv` identifies the external normalized IL dumps.
- `source-manifest.tsv` identifies external ILSpy decompilation files.
- `external-artifact-manifest.tsv` binds the complete external report, CIL, and
  decompiled-source set.
- `determinism-report.md` records the two-run comparison result.
- `asset-ripper-export-manifest.tsv` hashes the complete external Unity export.
- `asset-ripper-census.tsv` records recovered asset groups, scenes, and maps.
- `asset-ripper-determinism.md` records the independent export comparison.

The authoritative managed behavior index is `../mono-method-index.tsv`. The
archive-level content inventory is `../content-census.tsv`. Semantic Unity
object and authored-content counts require the separate serialized-asset audit
and must not be inferred from archive entry counts alone.
