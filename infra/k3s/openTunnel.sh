#!/usr/bin/env bash
# Opens the ssh tunnel to the Kubernetes API of the production server in the background
# (issue 93, task 93.2): 127.0.0.1:6443 of the job goes to 127.0.0.1:6443 of the server, as the Linux
# user tunnel, which may forward that one port and has no shell. The API itself stays closed in ufw.
# Needs TUNNEL_SSH_PRIVATE_KEY (base64 of the private key), PRODUCTION_SSH_KNOWN_HOSTS and
# PRODUCTION_SERVER_HOST.
set -Eeuo pipefail

for name in TUNNEL_SSH_PRIVATE_KEY PRODUCTION_SSH_KNOWN_HOSTS PRODUCTION_SERVER_HOST; do
  if [[ -z "${!name:-}" ]]; then
    echo "openTunnel: $name is missing" >&2
    exit 1
  fi
done

dir="$(mktemp -d)"
chmod 700 "$dir"
umask 077
printf '%s' "$TUNNEL_SSH_PRIVATE_KEY" | base64 -d > "$dir/key"
printf '%s\n' "$PRODUCTION_SSH_KNOWN_HOSTS" > "$dir/known_hosts"
chmod 600 "$dir/key" "$dir/known_hosts"

ssh -f -N \
  -i "$dir/key" \
  -o IdentitiesOnly=yes \
  -o BatchMode=yes \
  -o ExitOnForwardFailure=yes \
  -o UserKnownHostsFile="$dir/known_hosts" \
  -o StrictHostKeyChecking=yes \
  -o ServerAliveInterval=30 \
  -L 6443:127.0.0.1:6443 \
  "tunnel@$PRODUCTION_SERVER_HOST"

echo "openTunnel: the Kubernetes API of production is on 127.0.0.1:6443"
