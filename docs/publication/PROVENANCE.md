# Provenance and dependencies

Good Pirate 1.13 is the recovered content and behavior authority. The original
APK SHA-256 is `eb30d068b2f811d5360b1d68dbb88d3969e4159ca3fd87223125a8b49e1773f7`.
Original game code, models, textures, maps, audio and other recovered material
retain their original ownership. This project is unofficial and is not
endorsed by the original developer. No blanket open-source license is granted
over recovered material.

Reconstruction services, platform adapters, compatibility shaders, tests and
publication tooling are identified by their paths under `Assets/Reconstruction`,
`Assets/Editor`, `Assets/Tests` and `tool`. Rendering and platform compatibility
work also incorporates user-authorized DOKDO reconstruction implementations.
The steering visual repairs the recovered rig at runtime.

The publication snapshot starts from the locally reviewed reconstruction
checkout, including the paid-content and clipboard-save repairs. Earlier local
development history, original APKs and raw reverse-engineering output are not
included. Normalized evidence indexes preserve method identity and hashes.

## DOTween

Free DOTween core and unmodified modules are pinned to the official repository
commit `bfb02b05551a45204352dc43bdc15b882147891a`:
https://github.com/Demigiant/dotween/tree/bfb02b05551a45204352dc43bdc15b882147891a

Copyright Daniele Giardini, Demigiant. Its original license and readme accompany
the package. The publication does not include the recovered DOTween Pro DLL.
Scene adapters are independently implemented against the free core.
The unmodified upstream core source is included under `ThirdParty/DOTween`.
The pinned compiled core SHA-256 is
`5cee7f2b16fd3ba509ecac8bfc95efc9347682956fc4c1efde14618176e34491`.

## Retired packages

`removed-payloads.tsv` records hashes of the removed SDK and replacement payloads.
Narrow local adapters preserve required callback and serialization contracts.
No billing, cloud, attribution, analytics, social or advertising service is
initialized. The three obscured primitive adapters retain the recovered field
layout so reconstruction clipboard saves remain readable.

Fonts, Unity packages and other recovered dependencies retain their respective
licenses and ownership. Public source availability does not establish new
redistribution rights for recovered third-party content.
