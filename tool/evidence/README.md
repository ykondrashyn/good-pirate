# Good Pirate 1.13 evidence pipeline

This pipeline accepts only the exact version 1.13 APK whose SHA-256 is
`eb30d068b2f811d5360b1d68dbb88d3969e4159ca3fd87223125a8b49e1773f7`.
It never writes to the APK.

Run the complete deterministic audit with:

```sh
./tool/evidence/run_twice.sh
```

The command performs two clean extractions, indexes every managed method by
assembly and metadata token, hashes normalized CIL bodies, decompiles the two
Good Pirate managed assemblies, and compares both evidence runs. It then runs
two independent pinned AssetRipper exports and compares their complete file
manifests. Only small indexes and manifests are published under `docs/`.

Bulky output is written outside Git to
`../good_pirate_1_13_evidence`. ILSpy `11.0.0.9375` is restored
through a local tool manifest using the isolated .NET SDK `10.0.100`. The
pinned ILSpy package is downloaded from the official NuGet feed only when it is
absent, then verified against the SHA-256 embedded in
`run_1_13_evidence.py`. The pipeline disables update checks and .NET CLI
telemetry.

AssetRipper `1.3.14` is downloaded from its official GitHub release only when
absent. Its pinned mac arm64 ZIP SHA-256 is
`680890b97c58be02075438814b3f41ec2a928ae0be8e94fa24ad51faa81078aa`.

The isolated .NET SDK must already exist at
`../good_pirate_1_13_evidence/tools/dotnet-10.0.100`.
Remaining local tools come from Unity `6000.3.18f1`: Mono, Mono.Cecil,
`ikdasm`, Android OpenJDK, `apksigner`, and LLVM `nm`. The package manifest
reader is the locally installed Android `apkanalyzer`.
