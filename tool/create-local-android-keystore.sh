#!/bin/zsh
set -euo pipefail

project_dir=${0:A:h:h}
unity_version=${UNITY_VERSION:-6000.3.18f1}
android_player=/Applications/Unity/Hub/Editor/$unity_version/PlaybackEngines/AndroidPlayer
java_home=${UNITY_JAVA_HOME:-$android_player/OpenJDK}
keystore_path=${GP113_RELEASE_KEYSTORE:-$project_dir/.secrets/android/release.keystore}
password_path=${GP113_RELEASE_PASSWORD_FILE:-$project_dir/.secrets/android/keystore.pass}
key_alias=${GP113_RELEASE_KEY_ALIAS:-good-pirate-113-reconstruction}
keytool=$java_home/bin/keytool

if [[ ! -x $keytool ]]; then
  print -u2 "Pinned keytool is unavailable: $keytool"
  exit 1
fi
if [[ -f $keystore_path && -f $password_path ]]; then
  "$keytool" -list -keystore "$keystore_path" \
    -storepass:file "$password_path" -alias "$key_alias" >/dev/null
  print "Using existing gitignored Android signing key: $keystore_path"
  exit 0
fi
if [[ -e $keystore_path || -e $password_path ]]; then
  print -u2 "Signing key state is incomplete, refusing to overwrite it"
  exit 1
fi

mkdir -p "${keystore_path:h}" "${password_path:h}"
umask 077
openssl rand -base64 -out "$password_path" 36
"$keytool" -genkeypair -noprompt \
  -keystore "$keystore_path" \
  -storetype PKCS12 \
  -storepass:file "$password_path" \
  -alias "$key_alias" \
  -keyalg RSA \
  -keysize 4096 \
  -sigalg SHA256withRSA \
  -validity 10000 \
  -dname "CN=Good Pirate 1.13 Reconstruction,OU=Personal Reconstruction,O=Offline,C=PL"
chmod 600 "$keystore_path" "$password_path"

print "Created gitignored Android signing key: $keystore_path"
print "Back up the keystore and password file together, neither is recoverable."
