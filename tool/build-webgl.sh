#!/bin/zsh
set -euo pipefail

project_dir=${0:A:h:h}
unity_editor=${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.3.18f1/Unity.app/Contents/MacOS/Unity}
build_path=${GP113_WEBGL_BUILD_PATH:-$project_dir/Builds/WebGL/dev}
log_path=$project_dir/Logs/webgl-build.log

mkdir -p "$project_dir/Logs" "$build_path"

"$unity_editor" \
  -batchmode \
  -nographics \
  -quit \
  -projectPath "$project_dir" \
  -executeMethod GoodPirate113.Editor.GoodPirate113Build.BuildWebGl \
  -gp113BuildPath "$build_path" \
  -logFile "$log_path"

manifest=$build_path/build-manifest.sha256
find "$build_path" -type f ! -name build-manifest.sha256 -print0 \
  | sort -z \
  | xargs -0 shasum -a 256 \
  | sed "s#  $build_path/#  #" > "$manifest"

tail -n 80 "$log_path"
printf 'Build manifest: %s\n' "$manifest"

