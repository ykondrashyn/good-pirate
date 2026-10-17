#!/bin/sh

set -u

mode=${1:-baseline}
case "$mode" in
  baseline|release)
    ;;
  *)
    printf '%s\n' 'usage: ./tool/validate/run-all.sh [baseline|release]' >&2
    exit 2
    ;;
esac

script_dir=$(CDPATH= cd -- "$(dirname -- "$0")" && pwd)
failure_count=0

for validator in \
  verify_apk.py \
  audit_repository.py \
  validate_ledgers.py \
  audit_offline.py \
  validate_unity.py \
  validate_reproducibility.py
do
  printf 'RUN\t%s\tmode=%s\n' "$validator" "$mode"
  if PYTHONDONTWRITEBYTECODE=1 python3 "$script_dir/$validator" "$mode"
  then
    :
  else
    failure_count=$((failure_count + 1))
  fi
done

if [ "$failure_count" -ne 0 ]
then
  printf 'GATE\tFAIL\tvalidators_failed=%s mode=%s\n' "$failure_count" "$mode"
  exit 1
fi

printf 'GATE\tPASS\tvalidators_failed=0 mode=%s\n' "$mode"
