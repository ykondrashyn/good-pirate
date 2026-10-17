# Version 1.13 authority policy

Version 1.13 is the only gameplay and content donor for this repository.
Cross-version similarity may be recorded as historical context, but no file,
constant, content count, serialized value, or behavior from another release may
enter the reconstruction unless the user explicitly changes the scope.

Decompiled C# is a readable representation of managed CIL. It is not claimed as
the original source. When decompiled syntax and CIL differ, normalized 1.13 CIL
wins. When code and serialized Unity state differ, the serialized 1.13 state
defines object layout and authored values, while the CIL defines runtime logic.

Every intentional offline replacement must retain the original reachable public
callback where practical and must be recorded in `docs/offline-substitutions.tsv`.

## User-authorized DOKDO compatibility exceptions

On 2026-08-28 the user explicitly authorized importing DOKDO implementations.
This repository interprets that permission as reusable rendering and platform
compatibility code where the 1.13 export contains a dummy body or an obsolete
platform API. It does not authorize DOKDO gameplay, scenes, progression data,
content counts, prefabs, saves, or service behavior.

The first accepted import replaces all eight AssetRipper dummy shader bodies
under `Assets/Shader`. The exact hashes are pinned in
`docs/validation/reproducibility-inputs.json`. Good Pirate 1.13 shader GUIDs,
names, and scene bindings remain authoritative.

The DOKDO water shader depends on a foam texture and tuned material properties
that were not represented by the dummy 1.13 shader export. The user separately
authorized importing the complete DOKDO water appearance on 2026-08-28. The
DOKDO foam texture and water tuning are therefore accepted rendering inputs.
The Good Pirate material GUID, shader GUID, texture GUIDs for recovered 1.13
inputs, and all scene references remain unchanged.

The second accepted compatibility use is the WebGL pointer-capture pattern. A
Good Pirate 1.13-specific adapter references the earlier private browser
compatibility implementation, converts browser coordinates into the fixed
authored canvas, routes held pointer samples through the recovered
`TouchPanel`, and releases on pointer-up, cancellation, focus loss, and scene
UI deactivation. The exact reference commit, path, and hash are pinned. The
recovered `TouchPad` and `ShipController` methods remain the sole authority for
steering strength, rotation, acceleration, and release. No synthetic release
tail or imported donor gameplay logic is used.

The third accepted compatibility use is DOKDO's separation of browser timing
from recovered gameplay. Good Pirate uses its own platform adapter and does not
copy DOKDO's forced 60 FPS value. On WebGL it selects Unity's browser-managed
frame rate so the main loop uses `requestAnimationFrame`. Android and Editor
retain Good Pirate 1.13's recovered 30 or 60 FPS preference. Physics timing is
unchanged.

The fourth accepted compatibility use repairs the recovered two-bone
`Sphere001` steering visual. The 1.13 mesh, material, texture, scene hierarchy,
bone transforms, and `TouchPad` behavior remain authoritative. At runtime a
Good Pirate adapter reconstructs the missing eight-ring semicircular root dome
on an in-memory mesh, then forces both recovered bone influences to render.
The source mesh asset remains byte-identical to the 1.13 AssetRipper export.
The joystick texture uses clamp sampling, no mipmaps, and uncompressed WebGL
RGBA32, matching DOKDO's accepted compatibility fix. No DOKDO steering formula,
controller state, scene object, or prefab is imported.

Every accepted donor input and its source Git blob are pinned. No donor Git
history, gameplay, scenes, prefabs, progression data, or services are copied.
