# Good Pirate v0.1.0-preview.1

An unofficial offline reconstruction preview based on Good Pirate 1.13.

- Play in a browser, with recovered production scenes, models, maps and audio.
- Hold and drag to steer. Use the recovered tutorial and island controls.
- Former paid products use free local claims, with permanent and daily guards.
- Settings provides Copy Save and Paste Save for portable reconstruction saves.
- Original online services and recovered SDK binaries are absent.

## Validation

Unity 6000.3.18f1 compilation succeeded. The current Edit Mode suite passed
55/55 tests and the publication tooling suite passed 5/5. A separate clean
clone compiled without the original APK or private evidence archive.

The owner requested tests of at most one minute. Short browser checks showed
Chromium startup, steering, early tutorial combat, Settings and clipboard
export. Firefox initialized WebGL 2. A physical Pixel 10 Pro XL running
Android 17 and Chrome 155 rendered the game and accepted Set Sail. Captured
Chromium and mobile logs had no uncaught errors or unexpected game requests.

## Limitations

This release has smoke coverage, not the original full acceptance. Complete
tutorial progression, 30-minute sessions, all product routes, upgrades, death
and recovery, browser save exchange, corruption recovery and full mobile
controls have not been accepted on this exact release. Earlier save-transfer
tests do not replace those release-build checks. Android APK acceptance and
full offline content parity remain incomplete.

Existing RectTransform parenting and IAP-object persistence warnings remain.
Keep a clipboard backup of progress. Browser storage is local to each origin.
Original localhost progress must be copied there and pasted in the hosted game.

Download the WebGL ZIP for local hosting. It must be served over HTTP or HTTPS,
not opened directly as a file. No Android APK is included in this release.
