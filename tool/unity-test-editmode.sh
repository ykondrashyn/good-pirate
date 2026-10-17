#!/bin/zsh
set -euo pipefail

project_dir=${0:A:h:h}
unity_editor=${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/6000.3.18f1/Unity.app/Contents/MacOS/Unity}
log_path="$project_dir/Logs/unity-editmode-tests.log"
results_path="$project_dir/Logs/editmode-results.xml"

mkdir -p "$project_dir/Logs"

"$unity_editor" \
  -batchmode \
  -nographics \
  -projectPath "$project_dir" \
  -runTests \
  -testPlatform EditMode \
  -testResults "$results_path" \
  -logFile "$log_path"

tail -n 80 "$log_path"
