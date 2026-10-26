#!/usr/bin/env bash
# Puts the secrets of production into k3s (issue 95, tasks 95.1 and 95.2). Terraform only names these
# Secrets, so no password is in a .tf file, a variable default or the Terraform state.
# Needs the masked and protected GitLab variables DATABASE_PASSWORD_PRODUCTION, REGISTRY_PULL_USER and
# REGISTRY_PULL_TOKEN, and a kubeconfig of the login gitlab-deployer in KUBECONFIG.
# Every value goes through a file with rights 600, never on a command line that ps can show, and the
# script never prints one.
set -Eeuo pipefail

for name in DATABASE_PASSWORD_PRODUCTION REGISTRY_PULL_USER REGISTRY_PULL_TOKEN; do
  if [[ -z "${!name:-}" ]]; then
    echo "applySecrets: $name is missing" >&2
    exit 1
  fi
done

namespace="connect-production"
registry="${CI_REGISTRY:-gitlab.fdmci.hva.nl:5050}"

umask 077
work="$(mktemp -d)"
trap 'rm -rf "$work"' EXIT

# apply_secret <name> [kubectl create secret arguments]: makes the Secret, or updates it when it exists.
apply_secret() {
  local name="$1"
  shift
  kubectl -n "$namespace" create secret generic "$name" "$@" --dry-run=client -o yaml \
    | kubectl -n "$namespace" apply -f -
}

# The database of production: its own user, database and password, never shared with the test site.
printf 'connect' > "$work/user"
printf 'connect' > "$work/db"
printf '%s' "$DATABASE_PASSWORD_PRODUCTION" > "$work/password"
apply_secret connect-database-secret \
  --from-file=POSTGRES_USER="$work/user" \
  --from-file=POSTGRES_DB="$work/db" \
  --from-file=POSTGRES_PASSWORD="$work/password"

# How the server and the migrations reach that database. Host is the Service connect-database.
printf 'Host=connect-database;Port=5432;Database=connect;Username=connect;Password=%s' \
  "$DATABASE_PASSWORD_PRODUCTION" > "$work/connection"
apply_secret connect-server-secret \
  --from-file=ConnectionStrings__DefaultConnection="$work/connection"

# The login k3s uses to pull the images, the deploy token connect-production-pull with only read_registry.
auth="$(printf '%s:%s' "$REGISTRY_PULL_USER" "$REGISTRY_PULL_TOKEN" | base64 -w0)"
printf '{"auths":{"%s":{"auth":"%s"}}}' "$registry" "$auth" > "$work/dockerconfig"
apply_secret connect-registry-pull \
  --type=kubernetes.io/dockerconfigjson \
  --from-file=.dockerconfigjson="$work/dockerconfig"
