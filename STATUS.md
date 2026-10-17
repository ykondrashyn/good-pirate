# Preview acceptance status

This source snapshot targets `v0.1.0-preview.1`. Acceptance belongs to a built
artifact, not to this status page. Consult the manifest and release notes for
the deployed version, source commit, file hashes and actual browser evidence.

The owner shortened testing to a maximum of one minute per test on 2026-10-17.
The first public artifact is consequently a smoke-tested preview. It does not
claim the original 30-minute or full-feature acceptance gate.

The captured release-build checks cover Chromium steering, early tutorial
combat, Settings and Copy Save. Firefox initialized a WebGL 2 canvas. Physical
Chrome on a Pixel 10 Pro XL rendered the game and accepted Set Sail. Mobile
steering, orientation, save transfer and all 16 purchase routes were not fully
accepted in this abbreviated pass. Captured Chromium and mobile runtime logs
contained no uncaught errors or unexpected application requests.

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
