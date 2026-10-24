#!/usr/bin/env bash
# Installs k3s on the production server the same way every time (issue 93, task 93.1).
# Why k3s: docs/decisions/010-productionOnK3s.md. How to run it and check it:
# docs/team/onboarding.md, section Production server.
# Run as root on the server, from the root of the repository: sudo infra/k3s/installK3s.sh
set -Eeuo pipefail

# One pinned version, never latest or a channel name. An update is a merge request that changes
# these three lines after reading the release notes of the new version.
# https://github.com/k3s-io/k3s/releases/tag/v1.36.5%2Bk3s1
# (released 30 September 2026, chosen on 24 October 2026)
K3S_VERSION="v1.36.5+k3s1"
# The install script of that same release. It is downloaded to a file and only runs when its
# sha256 matches, never piped into sh.
INSTALL_SCRIPT_URL="https://raw.githubusercontent.com/k3s-io/k3s/v1.36.5%2Bk3s1/install.sh"
INSTALL_SCRIPT_SHA256="46177d4c99440b4c0311b67233823a8e8a2fc09693f6c89af1a7161e152fbfad"

# The pod and service networks of k3s, which must reach the server itself through ufw.
# https://docs.k3s.io/installation/requirements#operating-systems, part UFW (read 24 October 2026)
POD_NETWORK="10.42.0.0/16"
SERVICE_NETWORK="10.43.0.0/16"

HERE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
CONFIG_TARGET="/etc/rancher/k3s/config.yaml"
TRAEFIK_TARGET="/var/lib/rancher/k3s/server/manifests/traefik-config.yaml"

say() { echo "installK3s: $*"; }
fail() {
  echo "installK3s: $*" >&2
  exit 1
}
trap 'echo "installK3s: stopped at line $LINENO, the step above did not finish. Fix the cause and run the script again." >&2' ERR

[[ "${EUID}" -eq 0 ]] || fail "run it as root: sudo $0"
command -v ufw >/dev/null || fail "ufw is missing. The firewall of the server (issue 238) comes first."
ufw status | grep -q '^Status: active' || fail "ufw is not active. The firewall of the server (issue 238) comes first."
[[ -f "$HERE/config.yaml" && -f "$HERE/traefikPorts.yaml" ]] || fail "config.yaml and traefikPorts.yaml must sit next to this script."

installed="none"
if command -v k3s >/dev/null; then
  installed="$(k3s --version | awk 'NR == 1 {print $3}')"
fi

# The install script is downloaded and checked before anything on the server changes, so a
# failed download or a wrong checksum leaves the server as it was.
if [[ "$installed" != "$K3S_VERSION" ]]; then
  workdir="$(mktemp -d)"
  trap 'rm -rf "$workdir"' EXIT
  curl -fsSL --retry 3 -o "$workdir/install.sh" "$INSTALL_SCRIPT_URL"
  echo "$INSTALL_SCRIPT_SHA256  $workdir/install.sh" | sha256sum --check --quiet \
    || fail "the install script does not match its sha256. Nothing was changed."
fi

config_changed=0
# copy_if_different <source> <target>: copies only when the content differs, so a second run
# changes nothing. Returns 0 when it wrote the file.
copy_if_different() {
  if [[ -f "$2" ]] && cmp -s "$1" "$2"; then
    say "$2 is up to date"
    return 1
  fi
  install -D -m 0600 "$1" "$2"
  say "wrote $2"
}

if copy_if_different "$HERE/config.yaml" "$CONFIG_TARGET"; then
  config_changed=1
fi
# k3s applies a changed file in its manifests folder by itself, also while it runs.
copy_if_different "$HERE/traefikPorts.yaml" "$TRAEFIK_TARGET" || true

# ufw skips a rule it already has. There is no rule for 6443 or 8081: both stay closed.
ufw allow from "$POD_NETWORK" to any comment 'k3s pods'
ufw allow from "$SERVICE_NETWORK" to any comment 'k3s services'

if [[ "$installed" == "$K3S_VERSION" ]]; then
  say "k3s $K3S_VERSION is already installed"
  if [[ "$config_changed" -eq 1 ]]; then
    say "config.yaml changed, restarting k3s"
    systemctl restart k3s
  fi
else
  say "installing k3s $K3S_VERSION (installed now: $installed)"
  INSTALL_K3S_VERSION="$K3S_VERSION" sh "$workdir/install.sh"
fi

say "waiting until the node is Ready"
for _ in $(seq 1 60); do
  [[ -n "$(k3s kubectl get nodes -o name 2>/dev/null)" ]] && break
  sleep 5
done
k3s kubectl wait --for=condition=Ready node --all --timeout=180s

say "waiting until Traefik runs with the settings of traefikPorts.yaml"
for _ in $(seq 1 60); do
  k3s kubectl -n kube-system get deployment traefik >/dev/null 2>&1 && break
  sleep 5
done
k3s kubectl -n kube-system rollout status deployment/traefik --timeout=300s

say "done"
k3s --version | head -n 1
k3s kubectl get nodes
ss -ltnp | grep -E 'traefik' || true
