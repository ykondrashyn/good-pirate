# AssetRipper 1.13 determinism

Two independent AssetRipper 1.3.14 processes exported the two clean APK
extractions to identical normalized file views. Raw exports are preserved and
their manifests differ because AssetRipper generates fresh GUID and
`timeCreated` values. Normalization replaces generated asset GUIDs with
path-derived identifiers, replaces other generated 32-character identifiers by
stable occurrence order within each file, and zeroes `timeCreated`.

- Exported files: 7115
- Serialized scenes: 3
- Resources maps: 16
- Run A export manifest SHA-256: `e90b5443bcb32c0b66dda888705791b98316c9bdb2803e22ca23670d340a8c8d`
- Run B export manifest SHA-256: `b213485ffc764805b1d0a316860bdf4500f29493ee014155fefbdf34ad68be53`
- Tool archive SHA-256: `680890b97c58be02075438814b3f41ec2a928ae0be8e94fa24ad51faa81078aa`

Both 313 MB exports remain outside Git under
`../good_pirate_1_13_evidence/asset-ripper`.
