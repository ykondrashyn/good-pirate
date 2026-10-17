# Good Pirate 1.13 Reconstruction

An unofficial offline reconstruction of Good Pirate 1.13 for Unity 6.
Recovered scenes, maps and managed gameplay are combined with reconstruction
services for browser controls, saves and local reward claims.

[Play preview](https://ykondrashyn.github.io/good-pirate/play/) ·
[Project homepage](https://ykondrashyn.github.io/good-pirate/) ·
[Releases](https://github.com/ykondrashyn/good-pirate/releases) ·
[Report a problem](https://github.com/ykondrashyn/good-pirate/issues)

The preview links become available when an accepted release is deployed.
This is a reconstruction preview, not a claim of full game parity.
See [STATUS.md](STATUS.md) for current acceptance and limitations.

## Play and save

Hold and drag on the sea to steer. Release to stop accelerating. Follow the
recovered tutorial and use the on-screen controls to manage your fleet.
Portrait presentation is preserved. Click or tap to enable browser audio.

Progress saves locally. In Settings, **Copy Save** exports a checksummed save
document. **Paste Save** asks for confirmation before replacing progress and
reloading. Invalid saves are rejected and a pre-import recovery copy is kept.
Transfer localhost progress by copying there and pasting in the hosted game.
Clearing browser storage can remove progress, so keep an exported backup.

Former paid products use free local claims. Permanent unlocks are guarded
against duplicate grants, daily offers retain their limits and currency
products are repeatable. No payment, advertising or original online service
is contacted.

## Build

Install Unity **6000.3.18f1** with **Web Build Support**. Open this directory as
a Unity project. Unity restores the pinned packages. The original APK, private
evidence archive and obsolete SDK installations are not required to compile.

On macOS:

```sh
./tool/unity-compile.sh
./tool/unity-test-editmode.sh
python3 tool/publication.py audit
./tool/build-publication.sh
python3 tool/publication.py candidate
python3 -m http.server 8795 --bind 127.0.0.1 --directory Builds/PublicTest
```

Open `http://127.0.0.1:8795/good-pirate/play/`. Set `UNITY_EDITOR` when the
editor is installed elsewhere. Publication builds require a clean committed
source checkout. GitHub Actions deploys already tested release artifacts and
does not need a Unity license or private build credentials.

## Identity and provenance

- Unity: `6000.3.18f1`, Built-in Render Pipeline, Gamma
- Package: `com.reconstruction.goodpirate113`
- Product: `Good Pirate 1.13 Reconstruction`
- Original authority: Good Pirate 1.13, Unity `2018.3.11f1`, Mono, ARMv7
- Recovered maps: 16
- First public target: WebGL preview, Android acceptance remains separate

See [provenance and dependencies](docs/publication/PROVENANCE.md) and the
[publication protocol](docs/publication/README.md). Original material retains
its original ownership. This project is not associated with or endorsed by
the original developer. No blanket open-source license covers recovered code
or assets.

The original APK and raw reverse-engineering artifacts remain outside this
repository. Historical evidence tools are optional research utilities, not
build prerequisites. Nothing here publishes to or changes `dokdo.online`.
