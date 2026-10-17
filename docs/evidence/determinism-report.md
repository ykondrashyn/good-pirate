# Good Pirate 1.13 evidence determinism

Two independent clean extractions of the exact APK produced identical normalized
evidence manifests.

- APK SHA-256: `eb30d068b2f811d5360b1d68dbb88d3969e4159ca3fd87223125a8b49e1773f7`
- External artifact manifest SHA-256: `a17bde6c128da0647065659dfd104a32bb6b0ac3e4accd510f3dcb01d9ef6d49`
- Compared external artifacts: 1743
- Managed assemblies: 99
- Indexed managed methods: 117084
- Methods with CIL bodies: 101039
- Methods with more than one CIL instruction: 99602
- Normalized assembly CIL dumps: 99
- Decompiled source files: 1636

The two run directories remain outside Git under
`../good_pirate_1_13_evidence/runs`. Bulky extracted files,
normalized CIL, and decompiled source are not copied into this repository.
