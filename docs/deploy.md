# Deploying the demo with Docker

The demo is hosted as a public, single-origin website on the ms01 NAS behind
the existing reverse proxy at `https://omnomnom.compilesoftware.nl/`. There are
two ways to run it:

- **[Single image](#single-image-recommended)** - one container with everything
  inside. Simplest to host; recommended for the NAS.
- **[docker-compose](#docker-compose-alternative)** - the same single image plus
  a health-check/auto-restart sidecar. Handy for DockHand or any compose host.

Either way the browser only ever talks to nginx, which serves the SvelteKit
static build and forwards `/api/*` to the gateway (stripping `/api`, mirroring
the Vite dev proxy). Every request is same-origin, so the gateway has no CORS
policy and runs plain HTTP; TLS is terminated upstream by the NAS reverse proxy.

## Single image (recommended)

The root `Dockerfile` builds one image that runs all three pieces under
supervisord:

- `OmNomNom.AllInOne` - all six NServiceBus message endpoints in one process.
- `CompositionGateway` - the ServiceComposer HTTP host on loopback `:8080`.
- nginx - serves the static site and proxies `/api/*` to the gateway.

A single `/data` volume holds everything: `/data/db` for the per-service SQLite
files (including the shared `checkout.db`) and `/data/transport` for the
file-based LearningTransport folder. Because all three pieces share one
filesystem there is no cross-container coordination.

```bash
docker build -t omnomnom:single .
docker run -d --name omnomnom -p 8088:80 -v omnomnom-data:/data omnomnom:single
```

Point the NAS reverse proxy for `omnomnom.compilesoftware.nl` at
`http://<nas-host>:8088`.

The local machine runs podman and the host runs Docker, so a redeploy means
building here, shipping the image over SSH, and restarting the container
there. `deploy/deploy.sh` does all of that in one shot. It takes the host's
ssh destination as its first argument (or via the `SSH_TARGET` environment
variable) and fails early if none is given:

```bash
./deploy/deploy.sh user@host            # build, ship, (re)start the container
./deploy/deploy.sh user@host --reseed   # same, but wipe the data volume first
./deploy/deploy.sh user@host --chaos    # also enable the chaos /debug/hang endpoint
```

Flags combine in any order. Image tag, container name, port and volume are
overridable via environment variables (see the header of the script).

To reset to a clean catalogue, recreate the container against a fresh volume
(each endpoint reseeds its database on startup):

```bash
docker rm -f omnomnom
docker volume rm omnomnom-data
docker run -d --name omnomnom -p 8088:80 -v omnomnom-data:/data omnomnom:single
```

## Health checks and self-healing

The `OmNomNom.AllInOne` host exposes two probes over HTTP on `:8081` (internal
to the container):

- `/health/live` - liveness: every endpoint is still processing messages. If an
  endpoint hangs, its heartbeat goes stale and this flips to `503`.
- `/health/ready` - readiness: every endpoint has finished warm-up.

The image declares a Docker `HEALTHCHECK` that hits `/health/live`. Docker only
*marks* a container unhealthy; it does not restart it. The docker-compose stack
therefore includes a `willfarrell/autoheal` sidecar that restarts any container
reporting unhealthy. (`deploy.sh` builds with `--format docker` rather than OCI
so the `HEALTHCHECK` survives into Docker; OCI silently drops it.)

## Reaching the internal ports

Only port 80 is published (as 8088). The gateway (`:8080`) and the AllInOne
health/chaos endpoints (`:8081`) are internal. `curl` is installed in the image,
so reach them from inside with `docker exec`:

```bash
# liveness / readiness
docker exec omnomnom curl -s http://localhost:8081/health/live ; echo
docker exec omnomnom curl -s http://localhost:8081/health/ready ; echo

# from your laptop, over SSH
ssh <host> 'docker exec omnomnom curl -s http://localhost:8081/health/live'
```

## Chaos testing the heartbeat

Deploy with `--chaos` (sets `OMNOMNOM_CHAOS=1`) to expose a
`POST /debug/hang/{endpoint}` that deliberately hangs an endpoint's message pump,
so you can watch the liveness heartbeat detect it and autoheal restart the
container:

```bash
./deploy/deploy.sh user@host --chaos
```

Trigger the hang and watch liveness flip:

```bash
# spray blocking messages onto Marketing's queue (default 200; ?count=N to change)
docker exec omnomnom curl -s -X POST http://localhost:8081/debug/hang/Marketing ; echo

# poll liveness - goes 503 within the heartbeat StaleAfter window (~30s)
docker exec omnomnom curl -s http://localhost:8081/health/live ; echo
```

Only Marketing stops; the host process and the other five endpoints stay up -
exactly the "hung but alive" failure the per-endpoint heartbeat exists to catch.
Recovery is a container restart (autoheal, or restart it by hand). `/debug/hang`
exists only when deployed with `--chaos`, so the public demo never exposes it.

## docker-compose (alternative)

`docker-compose.yml` runs the prebuilt single image as a stack and adds a health
check plus automatic restart-on-unhealthy - handy for DockHand or any
compose-based host:

| Service    | Image                  | Role |
|------------|------------------------|------|
| `omnomnom` | `omnomnom:single`      | the whole demo (endpoints + gateway + nginx) |
| `autoheal` | `willfarrell/autoheal` | restarts `omnomnom` when its health check reports unhealthy |

It references the prebuilt image rather than building, so build and load it first
(`deploy.sh` does this, or `docker build` locally), then:

```bash
docker compose up -d
```

The `omnomnom` service publishes host port 8088, mounts a single `data` volume at
`/data`, and its health check hits the AllInOne liveness probe. Because Docker
does not restart an unhealthy container on its own, the `autoheal` service (which
needs the Docker socket) does. If your host UI already restarts unhealthy
containers, remove the `autoheal` service.

Enable chaos by adding `OMNOMNOM_CHAOS=1` to the `omnomnom` service's
`environment:`. Reseed by wiping the data volume:

```bash
docker compose down
docker volume rm omnomnom_data
docker compose up -d
```

Volume names are prefixed with the compose project name (the repo folder,
`omnomnom`), so the `data` volume is `omnomnom_data`. Run `docker volume ls` to
confirm the exact name on the host.
