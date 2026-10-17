#!/bin/zsh
set -euo pipefail

project_dir=${0:A:h:h}
build_path=${GP113_WEBGL_BUILD_PATH:-$project_dir/Builds/WebGL/dev}
if (( $# > 1 )); then
  print -u2 "usage: $0 [port]"
  exit 2
fi
port=${1:-${GP113_WEBGL_PORT:-8766}}

if [[ ! -f "$build_path/index.html" ]]; then
  print -u2 "WebGL build is missing at $build_path"
  exit 1
fi

exec python3 -m http.server "$port" --bind 127.0.0.1 --directory "$build_path"
