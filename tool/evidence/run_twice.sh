#!/bin/zsh
set -euo pipefail

script_dir="${0:A:h}"
python3 "$script_dir/run_1_13_evidence.py" --run-id run-a
python3 "$script_dir/run_1_13_evidence.py" --run-id run-b
python3 "$script_dir/verify_1_13_evidence.py"
python3 "$script_dir/run_asset_ripper_1_13.py" --run-id run-a --port 18113
python3 "$script_dir/run_asset_ripper_1_13.py" --run-id run-b --port 18114
python3 "$script_dir/verify_asset_ripper_1_13.py"
