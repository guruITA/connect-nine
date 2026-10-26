#!/bin/sh
# canaryCompare.sh <old address> <new address> <paths file>: asks the old and the new version every
# address of the paths file and compares the status and the kind of content (task 96.4, #1558). Then it
# checks that the new version says healthy and that the home page of the new version has its root
# element and loads every file it names. Runs inside k3s in the curl image, so it is plain sh, and it
# only sends GET. Ends with exit 1 when anything differs, after it has printed every line.
set -eu

old="$1"
new="$2"
paths="$3"
work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT
fail=0

# ask <method> <url> <body file>: prints "<status> <media type>", the media type without its charset.
ask() {
  answer="$(curl --silent --max-time 10 --request "$1" --output "$3" --write-out '%{http_code} %{content_type}' "$2" || echo "000")"
  code="${answer%% *}"
  type="$(printf '%s' "${answer#"$code"}" | cut -d';' -f1 | tr -d ' ')"
  printf '%s %s' "$code" "${type:-none}"
}

while read -r method path; do
  case "$method" in '' | '#'*) continue ;; esac
  a="$(ask "$method" "$old$path" "$work/old")"
  b="$(ask "$method" "$new$path" "$work/new")"
  if [ "$a" = "$b" ]; then
    echo "same       $method $path: $b"
  else
    echo "DIFFERENT  $method $path: old $a, new $b"
    fail=1
  fi
done < "$paths"

# The new version says it is healthy, with the whole answer, because "unhealthy" holds the word too.
if [ "$(ask GET "$new/api/health" "$work/health")" = "200 application/json" ] && grep -q '"status":"healthy"' "$work/health"; then
  echo "healthy    GET /api/health of the new version"
else
  echo "NOT HEALTHY  GET /api/health of the new version: $(head -c 200 "$work/health")"
  fail=1
fi

# The home page of the new version has the root element React draws in, and every file it names loads.
ask GET "$new/" "$work/home" > /dev/null
if grep -q '<div id="root">' "$work/home"; then
  echo "root       the home page of the new version has its root element"
else
  echo "NO ROOT    the home page of the new version has no root element"
  fail=1
fi
grep -o -E '(src|href)="/assets/[^"]+"' "$work/home" | cut -d'"' -f2 > "$work/files" || true
while read -r file; do
  code="$(curl --silent --max-time 10 --output /dev/null --write-out '%{http_code}' "$new$file" || echo "000")"
  if [ "$code" = "200" ]; then
    echo "loads      $file: 200"
  else
    echo "MISSING    $file: $code"
    fail=1
  fi
done < "$work/files"
if [ ! -s "$work/files" ]; then
  echo "NO FILES   the home page of the new version names no file under /assets"
  fail=1
fi

if [ "$fail" -eq 0 ]; then
  echo "canaryCompare: the new version answers the same as the old one"
else
  echo "canaryCompare: the new version answers differently, see the lines in capitals"
fi
exit "$fail"
