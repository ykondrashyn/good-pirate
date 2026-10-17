#!/bin/zsh
set -euo pipefail

project_dir=${0:A:h:h}
unity_version=${UNITY_VERSION:-6000.3.18f1}
unity_editor=${UNITY_EDITOR:-/Applications/Unity/Hub/Editor/$unity_version/Unity.app/Contents/MacOS/Unity}
android_player=/Applications/Unity/Hub/Editor/$unity_version/PlaybackEngines/AndroidPlayer
sdk_root=${UNITY_ANDROID_SDK_ROOT:-$android_player/SDK}
java_home=${UNITY_JAVA_HOME:-$android_player/OpenJDK}
build_tools_root=$sdk_root/build-tools/36.0.0
aapt=$build_tools_root/aapt
apksigner=$build_tools_root/apksigner
zipalign=$build_tools_root/zipalign
readelf=${READELF_PATH:-$android_player/NDK/toolchains/llvm/prebuilt/darwin-x86_64/bin/llvm-readelf}
untrusted_apk=$project_dir/Builds/Android/.intermediates/good-pirate-1.13-r1-untrusted.apk
aligned_apk=$project_dir/Builds/Android/.intermediates/good-pirate-1.13-r1-aligned.apk
release_apk=$project_dir/Builds/Android/good-pirate-1.13-r1-checkpoint.apk
log_path=$project_dir/Logs/android-checkpoint-build.log
report=$project_dir/.artifacts/android-checkpoint-audit.json
checksum=$project_dir/.artifacts/good-pirate-1.13-r1-checkpoint.sha256
keystore=${GP113_RELEASE_KEYSTORE:-$project_dir/.secrets/android/release.keystore}
password_file=${GP113_RELEASE_PASSWORD_FILE:-$project_dir/.secrets/android/keystore.pass}
key_alias=${GP113_RELEASE_KEY_ALIAS:-good-pirate-113-reconstruction}
export JAVA_HOME=$java_home
export PATH=$java_home/bin:$build_tools_root:$sdk_root/platform-tools:$PATH

for executable in "$unity_editor" "$aapt" "$apksigner" "$zipalign" \
  "$java_home/bin/java" "$java_home/bin/keytool" "$readelf" /usr/bin/python3
do
  if [[ ! -x $executable ]]; then
    print -u2 "Required Android checkpoint tool is unavailable: $executable"
    exit 1
  fi
done

available_kib=$(df -Pk "$project_dir" | awk 'NR == 2 {print $4}')
required_kib=$((12 * 1024 * 1024))
if (( available_kib < required_kib )); then
  print -u2 "Android checkpoint build requires 12 GiB free"
  exit 1
fi

cd "$project_dir"
mkdir -p "$project_dir/Logs" "${untrusted_apk:h}" "$project_dir/.artifacts"
state_dir=$(mktemp -d "$project_dir/.artifacts/android-state.XXXXXX")
state_files=(
  "Assets/00. Scenes/Intro/LightingData.asset"
  "Assets/00. Scenes/Loading/LightingData.asset"
  "Assets/00. Scenes/MainGame/LightingData.asset"
  Assets/Mesh/Sphere001.asset
  Assets/Texture2D/UI_ring003.png.meta
  ProjectSettings/DynamicsManager.asset
  ProjectSettings/EditorBuildSettings.asset
  ProjectSettings/GraphicsSettings.asset
  ProjectSettings/ProjectSettings.asset
  ProjectSettings/QualitySettings.asset
  ProjectSettings/UnityConnectSettings.asset
)
for state_file in $state_files
do
  mkdir -p "$state_dir/${state_file:h}"
  cp -p "$project_dir/$state_file" "$state_dir/$state_file"
done
state_restored=0
restore_project_state() {
  if (( state_restored == 0 )); then
    for state_file in $state_files
    do
      cp -p "$state_dir/$state_file" "$project_dir/$state_file"
    done
    state_restored=1
  fi
}
trap restore_project_state EXIT

for validator in \
  validate_ledgers.py \
  validate_unity.py \
  validate_reproducibility.py
do
  PATH=/usr/bin:/bin:/usr/sbin:/sbin \
    /usr/bin/python3 "tool/validate/$validator" baseline
done
if PATH=/usr/bin:/bin:/usr/sbin:/sbin \
  /usr/bin/python3 tool/validate/audit_offline.py baseline \
    > "$project_dir/.artifacts/offline-known-limitations.log" 2>&1
then
  print "Offline source audit passed"
else
  print "Offline source audit still has known checkpoint limitations"
  print "See: $project_dir/.artifacts/offline-known-limitations.log"
fi
./tool/unity-compile.sh
./tool/unity-test-editmode.sh

"$unity_editor" \
  -batchmode \
  -nographics \
  -quit \
  -projectPath "$project_dir" \
  -executeMethod GoodPirate113.Editor.GoodPirate113Build.BuildAndroidCheckpoint \
  -gp113BuildPath "$untrusted_apk" \
  -logFile "$log_path"
if ! grep -q "GP113_ANDROID_CHECKPOINT result=Succeeded" "$log_path"
then
  print -u2 "Unity did not report a successful Good Pirate Android build"
  exit 1
fi
if grep -q "Shader error in" "$log_path"
then
  print -u2 "Android checkpoint contains a shader compilation error"
  exit 1
fi
restore_project_state

./tool/create-local-android-keystore.sh
if [[ ! -f $untrusted_apk || ! -f $keystore || ! -f $password_file ]]; then
  print -u2 "Android checkpoint or signing material is missing"
  exit 1
fi

"$zipalign" -f -P 16 4 "$untrusted_apk" "$aligned_apk"
"$apksigner" sign \
  --ks "$keystore" \
  --ks-key-alias "$key_alias" \
  --ks-pass "file:$password_file" \
  --v1-signing-enabled false \
  --v2-signing-enabled true \
  --v3-signing-enabled true \
  --v4-signing-enabled false \
  --alignment-preserved true \
  --out "$release_apk" \
  "$aligned_apk"

/usr/bin/python3 tool/audit-android-apk.py \
  --apk "$release_apk" \
  --aapt "$aapt" \
  --apksigner "$apksigner" \
  --zipalign "$zipalign" \
  --readelf "$readelf" \
  --output "$report"
shasum -a 256 "$release_apk" | tee "$checksum"

print "Good Pirate checkpoint APK: $release_apk"
print "Audit report: $report"
print "Checksum: $checksum"
