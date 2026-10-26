#!/usr/bin/env bash
# canaryCheck.sh <image tag>: starts the new version beside the old one as connect-server-canary, which
# gets no visitors, and checks it against the old one before production switches (task 96.4, #1558).
# The canary is a copy of the Deployment connect-server with only its name, labels and image changed, so
# it runs with the same settings, Secrets, probes and limits. A Job with a pinned curl image runs
# canaryCompare.sh inside k3s against both. The canary must also not restart and log no Error or
# Critical while it is checked. Everything it made is removed at the end, also when the check fails.
# Runs in deploy-production with the login gitlab-deployer, after the migrations of the new version.
set -Eeuo pipefail

tag="${1:?usage: canaryCheck.sh <image tag>}"
here="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
ready_timeout="${CANARY_READY_TIMEOUT:-5m}"
paths_file="${CANARY_PATHS:-$here/canaryPaths.txt}"
# https://hub.docker.com/r/curlimages/curl, tag 8.23.0 of 20 October 2026, read on 26 October 2026.
curl_image="curlimages/curl:8.23.0"

k() { kubectl -n connect-production "$@"; }
cleanup() {
  k delete deployment/connect-server-canary service/connect-server-canary job/connect-canary-check \
    configmap/connect-canary-check --ignore-not-found --wait=false > /dev/null || true
}
trap cleanup EXIT
cleanup

if ! k get deployment connect-server > /dev/null 2>&1; then
  echo "canaryCheck: no old version runs yet, so there is nothing to compare with"
  exit 0
fi
old_image="$(k get deployment connect-server -o jsonpath='{.spec.template.spec.containers[0].image}')"
new_image="${old_image%:*}:$tag"
echo "canaryCheck: the old version runs $old_image, the canary gets $new_image"

k get deployment connect-server -o json | jq --arg image "$new_image" '{
  apiVersion, kind,
  metadata: {name: "connect-server-canary", namespace: .metadata.namespace,
             labels: {app: "connect-server-canary", "app.kubernetes.io/part-of": "connect"}},
  spec: (.spec
    | .replicas = 1
    | .selector.matchLabels = {app: "connect-server-canary"}
    | .template.metadata = {labels: {app: "connect-server-canary"}}
    | .template.spec.containers[0].image = $image)
}' | k apply -f -
k create service clusterip connect-server-canary --tcp=80:8080 --dry-run=client -o yaml | k apply -f -

if ! k rollout status deployment/connect-server-canary --timeout="$ready_timeout"; then
  echo "canaryCheck: the new version never got ready within $ready_timeout. Production stays on $old_image." >&2
  k get pods -l app=connect-server-canary >&2 || true
  exit 1
fi

since="$(date -u +%Y-%m-%dT%H:%M:%SZ)"
k create configmap connect-canary-check --from-file=canaryCompare.sh="$here/canaryCompare.sh" \
  --from-file=canaryPaths.txt="$paths_file" --dry-run=client -o yaml | k apply -f -
k apply -f - << YAML
apiVersion: batch/v1
kind: Job
metadata:
  name: connect-canary-check
  labels:
    app.kubernetes.io/part-of: connect
spec:
  backoffLimit: 0
  template:
    spec:
      restartPolicy: Never
      # The image runs as curl_user, uid 100. Kubernetes only accepts runAsNonRoot with a number.
      securityContext:
        runAsNonRoot: true
        runAsUser: 100
        runAsGroup: 101
      containers:
        - name: compare
          image: $curl_image
          command: ["sh", "/check/canaryCompare.sh", "http://connect-server", "http://connect-server-canary", "/check/canaryPaths.txt"]
          resources:
            requests: {memory: 16Mi, cpu: 10m}
            limits: {memory: 64Mi, cpu: 200m}
          volumeMounts:
            - {name: check, mountPath: /check}
      volumes:
        - name: check
          configMap: {name: connect-canary-check}
YAML

result=""
for _ in $(seq 1 90); do
  if [[ "$(k get job connect-canary-check -o jsonpath='{.status.succeeded}')" == "1" ]]; then
    result="passed"
    break
  fi
  if [[ "$(k get job connect-canary-check -o jsonpath='{.status.failed}')" == "1" ]]; then
    result="failed"
    break
  fi
  sleep 2
done
k logs job/connect-canary-check || true

restarts="$(k get pods -l app=connect-server-canary -o jsonpath='{.items[*].status.containerStatuses[*].restartCount}')"
errors="$(k logs deployment/connect-server-canary --since-time="$since" | grep -c -E '"LogLevel":"(Error|Critical)"' || true)"
echo "canaryCheck: while it was checked the canary restarted ${restarts:-0} times and logged $errors lines of Error or Critical"

if [[ "$result" == "passed" && "${restarts:-0}" == "0" && "$errors" == "0" ]]; then
  echo "canaryCheck: passed, production may switch to $new_image"
  exit 0
fi
echo "canaryCheck: failed (${result:-no answer from the check}). Production stays on $old_image." >&2
exit 1
