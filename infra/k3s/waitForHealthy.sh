#!/usr/bin/env bash
# waitForHealthy.sh <url>: asks the health address every 10 seconds, at most 12 times, until it answers
# 200 with "healthy" (issue 96, task 96.1). After 12 tries it prints the last answer and ends with exit 1,
# so the job deploy-production turns red when the new version never answers healthy.
set -Eeuo pipefail

url="${1:?usage: waitForHealthy.sh <url>}"
body="$(mktemp)"
trap 'rm -f "$body"' EXIT

code=""
for try in $(seq 1 12); do
  code="$(curl --silent --max-time 5 --output "$body" --write-out '%{http_code}' "$url" || true)"
  # The answer is {"status":"healthy"}. "unhealthy" also contains the word, so the check holds the quotes.
  if [[ "$code" == "200" ]] && grep -q '"status":"healthy"' "$body"; then
    echo "waitForHealthy: $url answered 200 healthy at try $try"
    exit 0
  fi
  if [[ "$try" -lt 12 ]]; then
    sleep 10
  fi
done

echo "waitForHealthy: no healthy answer from $url after 12 tries. The last answer: HTTP $code $(head -c 300 "$body")" >&2
exit 1
