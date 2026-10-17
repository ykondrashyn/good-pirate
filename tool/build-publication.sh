#!/bin/zsh
set -euo pipefail
project_dir=${0:A:h:h}
unity_editor=${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.3.18f1/Unity.app/Contents/MacOS/Unity}
build_path=$project_dir/Builds/WebGL/publication
mkdir -p "$project_dir/Logs"
if [[ -n "$(git -C "$project_dir" status --porcelain --untracked-files=normal)" ]]; then
  print -u2 'Commit the reviewed publication source before building'
  exit 1
fi
source_commit=$(git -C "$project_dir" rev-parse HEAD)
"$unity_editor" -batchmode -nographics -quit -projectPath "$project_dir" \
  -executeMethod GoodPirate113.Editor.GoodPirate113Build.BuildWebGlPublication \
  -gp113BuildPath "$build_path" -logFile "$project_dir/Logs/publication-build.log"
if [[ "$source_commit" != "$(git -C "$project_dir" rev-parse HEAD)" || -n "$(git -C "$project_dir" status --porcelain --untracked-files=normal)" ]]; then
  print -u2 'Source changed while building, refuse publication provenance'
  exit 1
fi
print -r -- "$source_commit" > "$build_path/source-commit.txt"
tail -n 20 "$project_dir/Logs/publication-build.log"
