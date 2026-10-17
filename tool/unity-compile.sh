#!/bin/zsh
set -euo pipefail

project_dir=${0:A:h:h}
unity_editor=${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.3.18f1/Unity.app/Contents/MacOS/Unity}
log_path="$project_dir/Logs/unity-compile.log"

mkdir -p "$project_dir/Logs"

"$unity_editor" \
  -batchmode \
  -nographics \
  -quit \
  -projectPath "$project_dir" \
  -logFile "$log_path"

tail -n 60 "$log_path"
