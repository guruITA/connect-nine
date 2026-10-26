#!/bin/sh
# Waits until Connect answers healthy, run by the job deploy-test after deploy/deployTest.sh (task 17.3, #313).
# Usage: sh deploy/waitForHealthy.sh https://31-70-66-186.sslip.io
# Green (exit 0) only when /api/health answers 200 with exactly "status":"healthy". A deploy that starts
# while the database is broken answers 503, and an old server that still runs counts for nothing either.
# Twelve tries ten seconds apart give the server and the database about two minutes to come up.
# It prints no variable and no secret: only the code and the body of the health answer, which by #311
# hold no exception, connection string or version.
set -eu

if [ "$#" -ne 1 ]; then
  echo "Usage: sh deploy/waitForHealthy.sh <address of the site, such as https://31-70-66-186.sslip.io>"
  exit 1
fi

address="${1%/}"
tries=12
pause=10

body=$(mktemp)
trap 'rm -f "$body"' EXIT

try=1
while :; do
  # Emptied first, so a try without an answer never shows the body of an earlier try.
  : > "$body"
  # 000 means no answer at all: nothing listens yet, or the five seconds ran out.
  code=$(curl --silent --max-time 5 --output "$body" --write-out '%{http_code}' "$address/api/health") || true

  if [ "$code" = "200" ] && grep -qF '"status":"healthy"' "$body"; then
    echo "Connect answers healthy (try $try of $tries)."
    exit 0
  fi

  echo "Try $try of $tries: /api/health answered $code, not healthy yet."
  if [ "$try" -ge "$tries" ]; then
    break
  fi
  try=$((try + 1))
  sleep "$pause"
done

echo "Connect did not answer healthy within about two minutes, so the test site is down."
echo "Last answer of /api/health: code $code, body:"
head -c 500 "$body"
echo
echo "Tell the team channel at once and follow Going back to the previous version in docs/team/onboarding.md."
exit 1
