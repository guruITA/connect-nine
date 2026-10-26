#!/usr/bin/env bash
# Writes a kubeconfig for one login of the pipeline into a new temporary folder and prints only its
# path (issue 93, task 93.2). It reads the CA of k3s from the file KUBE_CA_CERT (a GitLab variable of
# type File) and the token from KUBE_TOKEN, and never prints the token.
# KUBE_SERVER is the address of the Kubernetes API, by default the end of the ssh tunnel of openTunnel.sh.
set -Eeuo pipefail

for name in KUBE_CA_CERT KUBE_TOKEN; do
  if [[ -z "${!name:-}" ]]; then
    echo "kubeconfigFromToken: $name is missing" >&2
    exit 1
  fi
done
[[ -f "$KUBE_CA_CERT" ]] || { echo "kubeconfigFromToken: KUBE_CA_CERT is not a file" >&2; exit 1; }

server="${KUBE_SERVER:-https://127.0.0.1:6443}"
dir="$(mktemp -d)"
chmod 700 "$dir"
config="$dir/kubeconfig"

umask 077
cat > "$config" <<YAML
apiVersion: v1
kind: Config
clusters:
  - name: connect
    cluster:
      server: $server
      certificate-authority-data: $(base64 -w0 < "$KUBE_CA_CERT")
users:
  - name: pipeline
    user:
      token: $KUBE_TOKEN
contexts:
  - name: connect-production
    context:
      cluster: connect
      user: pipeline
      namespace: connect-production
current-context: connect-production
YAML

echo "$config"
