# Deploying the demo with Docker

The demo is hosted as a public, single-origin website on the ms01 NAS behind
the existing reverse proxy at `https://omnomnom.compilesoftware.nl/`. There are
two ways to run it:

- **[Single image](#single-image-recommended)** - one container with everything
  inside. Simplest to host; recommended for the NAS.
- **[docker-compose](#docker-compose-alternative)** - three separate containers.
  Useful locally when you want each piece in its own process.

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

To reset to a clean catalogue, recreate the container against a fresh volume
(each endpoint reseeds its database on startup):

```bash
docker rm -f omnomnom
docker volume rm omnomnom-data
docker run -d --name omnomnom -p 8088:80 -v omnomnom-data:/data omnomnom:single
```

## docker-compose (alternative)

`docker-compose.yml` runs the same three pieces as separate containers:

| Service    | Image base            | Role |
|------------|-----------------------|------|
| `allinone` | `dotnet/runtime:10.0` | `OmNomNom.AllInOne` - the six message endpoints |
| `gateway`  | `dotnet/aspnet:10.0`  | `CompositionGateway` - plain HTTP on `:8080` |
| `web`      | `nginx:alpine`        | Static site + `/api` proxy to `gateway:8080` |

Here `allinone` and `gateway` are separate processes, so they share two named
volumes - `transport` (the send-only gateway drops command files the endpoints
pick up) and `db` (the SQLite files, including `checkout.db`).

Storage paths in both variants are pinned through `OMNOMNOM_TRANSPORT_DIR` and
`OMNOMNOM_DB_DIR` (set in the Dockerfiles). Unset locally, the app keeps its
solution-relative dev defaults.

```bash
docker compose build
docker compose up -d
```

The `web` container publishes host port `8088`, same as above.

Reseed by wiping both volumes (drops the SQLite files and in-flight messages
together, so no message references a row that no longer exists):

```bash
docker compose down
docker volume rm omnomnom_db omnomnom_transport
docker compose up -d
```

Volume names are prefixed with the compose project name (the repo folder,
`omnomnom`). Run `docker volume ls` to confirm the exact names on the host.
