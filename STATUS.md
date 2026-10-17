# Preview acceptance status

This source snapshot targets `v0.1.0-preview.1`. Acceptance belongs to a built
artifact, not to this status page. Consult the manifest and release notes for
the deployed version, source commit, file hashes and actual browser evidence.

## Current verified foundation

- Recovered Loading, MainGame and Intro scenes compile in Unity 6000.3.18f1.
- All 16 recovered map resources are retained.
- The publication checkout removes recovered online SDK binaries and settings.
- Free DOTween core is pinned to an official upstream commit. Local adapters
  preserve scene-facing tween and retired service contracts.
- Narrow obscured-value adapters retain the reconstruction save field layout.
- The current Edit Mode suite contains 55 tests and passed after dependency
  replacement. Subsequent source changes must rerun this suite.
- Paid content has 16 local product routes. Browser acceptance must exercise
  them and verify one-time and daily guards.

## Required release evidence

- The exact release build must pass the documented 30-minute gameplay session.
- Chromium, Firefox and a physical mobile browser must have recorded acceptance.
- Clipboard origin transfer and all product routes require release-build evidence.
- Console and runtime request captures must pass before deployment.

Full offline content completeness and Android parity are not established by
this preview. Remaining warnings and gameplay limitations must be listed in
the actual release notes, not inferred from compilation or static checks.

The publication tools refuse to package an accepted preview without passing
evidence for the required scenarios. No production DOKDO deployment is involved.
