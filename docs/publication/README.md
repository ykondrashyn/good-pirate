# Publication workflow

Use Unity 6000.3.18f1 with Web Build Support. No original APK, evidence archive
or online SDK is needed to compile this source snapshot.

1. Run `./tool/unity-compile.sh` and `./tool/unity-test-editmode.sh`.
2. Run `python3 tool/publication.py audit`.
3. Commit the reviewed source and run `./tool/build-publication.sh`.
4. Run `python3 tool/publication.py candidate` to assemble a local test site.
5. Serve `Builds/PublicTest` and open `/good-pirate/play/`.
6. Complete the acceptance protocol and record actual results in an acceptance
   JSON file outside Git. No unexecuted scenario may be recorded as passed.
7. Run `python3 tool/publication.py package --acceptance FILE`.
8. Tag the tested source, create the GitHub prerelease and upload the generated
   ZIP, publication manifest and checksum file from `Builds/Release`.
9. Run the `Publish accepted preview` Actions workflow with the exact release
   tag. It verifies provenance and packages the accepted release for Pages.

Rollback uses the same workflow with the previous accepted tag. Never retarget
a published release tag or silently replace a release archive.

## Acceptance protocol

Record browser, platform, start and end UTC, actual interaction duration,
screenshots or recording references, console output and request inventory.
Required scenarios are fresh startup and tutorial, steering and cancellation,
docking, combat and rewards, upgrade, death and recovery, modal close paths,
all 16 local product routes, clipboard export and import, cancellation,
corruption rejection, backup recovery, reload persistence and origin transfer.

Desktop Chromium, desktop Firefox and a real mobile browser are required.
Mobile emulation is supplementary and cannot replace the physical browser.
The gameplay session must contain at least 30 minutes of actual interaction.
`tool/publication.py package` refuses incomplete or failed acceptance records.

Preview acceptance does not mean full offline reconstruction or Android parity.

## Short smoke preview

The owner may explicitly shorten testing. The 2026-10-17 instruction limits
tests to one minute. Such a record uses `level: smoke`, includes that explicit
policy and discloses incomplete scenarios. The package and homepage then say
`smoke-tested-preview`. Full acceptance remains a separate gate.

Publication documentation or tooling may change after the Unity build. The
packager permits this only when the complete `Assets`, `Packages` and
`ProjectSettings` Git trees still match the recorded build commit. The manifest
records both the exact Unity build commit and the publication-tooling commit.
The release tag identifies the Unity build commit. This avoids rebuilding or
retesting unchanged gameplay merely to update publication evidence.
