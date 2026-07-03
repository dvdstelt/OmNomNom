#!/usr/bin/env bash
#
# Build the single-image demo locally and ship it to the deployment host.
#
# The local machine runs podman; the deployment host runs Docker.
# `podman save` tags the image `localhost/omnomnom:single`, so after
# loading on the host we retag it to the short name the run command
# expects.
#
# The container is labelled autoheal=true and gets --restart unless-stopped, so a
# host-wide `willfarrell/autoheal` (or the host's own restart-on-unhealthy) can
# recreate it when the HEALTHCHECK reports unhealthy - Docker's own restart policy
# never acts on health status. Pass --autoheal to also start a dedicated sidecar
# for hosts that run no autoheal of their own.
#
# Usage:
#   ./deploy/deploy.sh <ssh-target> [--reseed] [--chaos]
#   SSH_TARGET=<ssh-target> ./deploy/deploy.sh [--reseed] [--chaos]
#
#   <ssh-target>  ssh destination of the deployment host (user@host, a
#                 bare host, or a Host alias from ~/.ssh/config). Required,
#                 either as the first argument or via SSH_TARGET.
#   --reseed      wipe the data volume before starting (drops the SQLite
#                 files and the LearningTransport queues together; every
#                 endpoint reseeds on startup)
#   --chaos       set OMNOMNOM_CHAOS=1 in the container, which exposes the
#                 chaos /debug/hang endpoint on the AllInOne host. Off by
#                 default; leave it off for the public demo.
#   --autoheal    also start a willfarrell/autoheal sidecar that restarts the
#                 container when the HEALTHCHECK reports unhealthy. Off by
#                 default: the container is always labelled autoheal=true and gets
#                 --restart unless-stopped, so a host-wide autoheal (or the host's
#                 own restart-on-unhealthy) can handle it. Pass this only when
#                 nothing else on the host does.
#
# Override any of these from the environment if the setup changes:
#   IMAGE       image tag built and run            (default omnomnom:single)
#   CONTAINER   container name on the host         (default omnomnom)
#   PORT        host port the reverse proxy hits   (default 8088)
#   VOLUME      named data volume on the host      (default omnomnom-data)

set -euo pipefail

SSH_TARGET="${SSH_TARGET:-}"
IMAGE="${IMAGE:-omnomnom:single}"
CONTAINER="${CONTAINER:-omnomnom}"
PORT="${PORT:-8088}"
VOLUME="${VOLUME:-omnomnom-data}"

RESEED=0
CHAOS=0
WITH_AUTOHEAL=0
for arg in "$@"; do
  case "$arg" in
    --reseed)
      RESEED=1
      ;;
    --chaos)
      CHAOS=1
      ;;
    --autoheal)
      WITH_AUTOHEAL=1
      ;;
    -*)
      echo "Unknown option: $arg" >&2
      exit 2
      ;;
    *)
      if [[ -z "$SSH_TARGET" ]]; then
        SSH_TARGET="$arg"
      else
        echo "Unexpected argument: $arg" >&2
        exit 2
      fi
      ;;
  esac
done

if [[ -z "$SSH_TARGET" ]]; then
  cat >&2 <<'USAGE'
Error: no SSH target given.

The deployment host's ssh destination is required, either as the first
argument or via the SSH_TARGET environment variable.

Usage:
  ./deploy/deploy.sh <ssh-target> [--reseed] [--chaos] [--autoheal]
  SSH_TARGET=<ssh-target> ./deploy/deploy.sh [--reseed] [--chaos] [--autoheal]

Examples:
  ./deploy/deploy.sh user@host
  ./deploy/deploy.sh user@host --reseed
  ./deploy/deploy.sh user@host --chaos
  ./deploy/deploy.sh user@host --autoheal

<ssh-target> is anything ssh accepts as a destination: user@host, a bare
host, or a Host alias from ~/.ssh/config.
USAGE
  exit 2
fi

# Run from the repo root regardless of where the script is invoked, so
# the build context (the root Dockerfile) resolves correctly.
cd "$(dirname "$0")/.."

echo ">> Building $IMAGE locally with podman"
# --format docker (not the default OCI) so the image carries the Dockerfile
# HEALTHCHECK; OCI silently drops it, and it would be lost on load into Docker.
podman build --format docker -t "$IMAGE" .

echo ">> Shipping $IMAGE to $SSH_TARGET"
podman save "$IMAGE" | ssh "$SSH_TARGET" 'docker load'

echo ">> Deploying on $SSH_TARGET"
# podman prefixes the saved image with localhost/; retag to the short
# name so `docker run $IMAGE` resolves locally instead of trying to pull
# from a registry.
ssh "$SSH_TARGET" bash -s -- "$IMAGE" "$CONTAINER" "$PORT" "$VOLUME" "$RESEED" "$CHAOS" "$WITH_AUTOHEAL" <<'REMOTE'
set -euo pipefail
IMAGE="$1"; CONTAINER="$2"; PORT="$3"; VOLUME="$4"; RESEED="$5"; CHAOS="$6"; WITH_AUTOHEAL="$7"

if docker image inspect "localhost/$IMAGE" >/dev/null 2>&1; then
  docker tag "localhost/$IMAGE" "$IMAGE"
fi

docker rm -f "$CONTAINER" >/dev/null 2>&1 || true

if [[ "$RESEED" == "1" ]]; then
  # Empty the mounted data rather than removing the volume: this clears the
  # transport folder (queues + audit + error) and the SQLite DBs together, and
  # works whether $VOLUME is a named volume or a bind-mounted host path. A
  # `docker volume rm` is a silent no-op for a bind mount, which is why messages
  # were surviving a reseed. A throwaway container mounts the same data and
  # deletes its contents; the app recreates /data/db and /data/transport on
  # startup and every endpoint reseeds.
  echo ">> Reseed: emptying $VOLUME (SQLite DBs + transport folder)"
  docker run --rm -v "${VOLUME}:/data" "$IMAGE" find /data -mindepth 1 -delete 2>/dev/null || true
fi

CHAOS_ENV=()
if [[ "$CHAOS" == "1" ]]; then
  echo ">> Chaos enabled: OMNOMNOM_CHAOS=1 (the /debug/hang endpoint is exposed)"
  CHAOS_ENV=(-e OMNOMNOM_CHAOS=1)
fi

docker run -d --name "$CONTAINER" \
  --restart unless-stopped \
  --label autoheal=true \
  -p "${PORT}:80" -v "${VOLUME}:/data" "${CHAOS_ENV[@]}" "$IMAGE"

# The container is labelled autoheal=true above, so a host-wide autoheal (or the
# host's own restart-on-unhealthy) can recreate it after the HEALTHCHECK's 3
# measured failures (interval 30s, retries 3). Docker's restart policy reacts only
# to a container *exiting*, never to health status, so nothing built into Docker
# does this by itself. --autoheal opts into a dedicated sidecar for hosts that run
# no autoheal of their own.
AUTOHEAL="${CONTAINER}-autoheal"
if [[ "$WITH_AUTOHEAL" == "1" ]]; then
  docker rm -f "$AUTOHEAL" >/dev/null 2>&1 || true
  docker run -d --name "$AUTOHEAL" --restart unless-stopped \
    -e AUTOHEAL_CONTAINER_LABEL=autoheal \
    -e AUTOHEAL_INTERVAL=10 \
    -e AUTOHEAL_START_PERIOD=90 \
    -v /var/run/docker.sock:/var/run/docker.sock \
    willfarrell/autoheal:latest >/dev/null
  echo ">> autoheal sidecar started ($AUTOHEAL); $CONTAINER restarts on unhealthy"
else
  echo ">> autoheal sidecar not started; $CONTAINER is labelled autoheal=true (pass --autoheal to add one, or let a host-wide autoheal handle it)"
fi

echo ">> Container status:"
docker exec "$CONTAINER" supervisorctl status || true
REMOTE

echo ">> Done. Point the reverse proxy at http://<host>:${PORT}"
