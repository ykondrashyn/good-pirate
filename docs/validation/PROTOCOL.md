# Independent validation protocol

This gate audits repository and release boundaries for the Good Pirate 1.13
reconstruction. It does not modify or extract the authoritative APK, write to
the evidence directory, or inspect the 1.16 project except for read-only file
fingerprint comparison.

## Commands

Run the development baseline from the repository root:

```sh
./tool/validate/run-all.sh
```

Run the stricter release-candidate gate only after creating a commit:

```sh
./tool/validate/run-all.sh release
```

The scripts require Python 3.10 or newer and use only the standard library.
Each finding is one tab-separated `PASS`, `FAIL`, `WARN`, or `SKIP` record. Any
`FAIL` makes the aggregate gate exit nonzero. Warnings remain visible but do not
make the baseline fail.

## Acceptance gates

| Area | Baseline requirement | Additional release requirement |
| --- | --- | --- |
| APK | Exact path, byte size, SHA-256, ZIP CRC, stable read identity, outside Git | Owner writes disabled or an immutable filesystem flag set |
| Repository | Independent Git root, visible tool and index sources, bounded paths, no escaping symlinks, no embedded recovery payloads, no known 1.16 marker or uncorroborated donor match | Existing commit and clean worktree |
| Ledgers | Exact column schemas, UTF-8, LF-only, fixed-width rows, unique valid IDs | Same |
| Offline boundary | No public endpoint, online package, direct network API, secret config, enabled analytics, cloud, ads, purchasing, or diagnostics | Same |
| Unity | Fixed editor, product, package, version, portrait settings, input mode, Android ABI, explicit SDKs, IL2CPP, package lock, and assembly identity | At least one enabled build scene |
| Reproducibility | Sorted input inventory, normalized text and JSON, required ignores, deterministic aggregate digest | Every normalized input tracked |

The APK validator proves that the file matched the declared identity and stayed
unchanged while it was read. Filesystem write protection is a separate release
condition. Changing permissions or flags on the original APK requires explicit
user authorization and is never performed by these scripts.

The donor audit catches known 1.16 identifiers and byte-identical files in
selected Unity asset and source formats. A shared file passes only when the same
relative path and bytes are independently present in the pinned 1.13 evidence.
This is a leakage detector, not proof of independent authorship. Behavioral
provenance still belongs in the ledgers.

The offline audit rejects public URLs even when they appear in disabled stock
Unity service settings. This keeps the checked-in release configuration free of
latent public endpoints. Loopback URLs are permitted.

## Gate scope

A passing result establishes repository hygiene and reproducible configuration.
It does not establish gameplay parity, successful Unity compilation, a WebGL
build, or physical Android acceptance. Those need separate build, automated
test, behavioral-oracle, and device evidence tied to the exact candidate commit.
