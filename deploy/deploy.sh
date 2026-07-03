#!/usr/bin/env bash
#
# Build the single-image demo locally and ship it to the deployment host.
#
# The local machine runs podman; the deployment host runs Docker.
# `podman save` tags the image `localhost/omnomnom:single`, so after
# loading on the host we retag it to the short name the run command
# expects.
#
# Usage:
#   ./deploy/deploy.sh <ssh-target> [--reseed]
#   SSH_TARGET=<ssh-target> ./deploy/deploy.sh [--reseed]
#
#   <ssh-target>  ssh destination of the deployment host (user@host, a
#                 bare host, or a Host alias from ~/.ssh/config). Required,
#                 either as the first argument or via SSH_TARGET.
#   --reseed      wipe the data volume before starting (drops the SQLite
#                 files and the LearningTransport queues together; every
#                 endpoint reseeds on startup)
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
for arg in "$@"; do
  case "$arg" in
    --reseed)
      RESEED=1
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
  ./deploy/deploy.sh <ssh-target> [--reseed]
  SSH_TARGET=<ssh-target> ./deploy/deploy.sh [--reseed]

Examples:
  ./deploy/deploy.sh user@host
  ./deploy/deploy.sh user@host --reseed

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
ssh "$SSH_TARGET" bash -s -- "$IMAGE" "$CONTAINER" "$PORT" "$VOLUME" "$RESEED" <<'REMOTE'
set -euo pipefail
IMAGE="$1"; CONTAINER="$2"; PORT="$3"; VOLUME="$4"; RESEED="$5"

if docker image inspect "localhost/$IMAGE" >/dev/null 2>&1; then
  docker tag "localhost/$IMAGE" "$IMAGE"
fi

docker rm -f "$CONTAINER" >/dev/null 2>&1 || true

if [[ "$RESEED" == "1" ]]; then
  echo ">> Reseed: removing volume $VOLUME"
  docker volume rm "$VOLUME" >/dev/null 2>&1 || true
fi

docker run -d --name "$CONTAINER" -p "${PORT}:80" -v "${VOLUME}:/data" "$IMAGE"
echo ">> Container status:"
docker exec "$CONTAINER" supervisorctl status || true
REMOTE

echo ">> Done. Point the reverse proxy at http://<host>:${PORT}"
