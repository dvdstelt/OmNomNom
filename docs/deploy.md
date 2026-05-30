# Deploying the demo with Docker

The repository ships a `docker-compose.yml` that runs the demo as a public,
single-origin website. It is intended for hosting on the ms01 NAS behind the
existing reverse proxy at `https://omnomnom.compilesoftware.nl/`.

## What runs

| Service    | Image base                  | Role |
|------------|-----------------------------|------|
| `allinone` | `dotnet/runtime:10.0`       | `OmNomNom.AllInOne` - all six NServiceBus message endpoints in one process |
| `gateway`  | `dotnet/aspnet:10.0`        | `CompositionGateway` - the ServiceComposer HTTP host, plain HTTP on `:8080` |
| `web`      | `nginx:alpine`              | Serves the SvelteKit static build and proxies `/api/*` to the gateway |

`allinone` and `gateway` share two named volumes:

- `transport` - the file-based LearningTransport folder. The send-only gateway
  drops command files here; the AllInOne endpoints pick them up.
- `db` - the per-service SQLite files, including the shared `checkout.db`.

Both paths are pinned through `OMNOMNOM_TRANSPORT_DIR` and `OMNOMNOM_DB_DIR`
(set in the Dockerfiles). Unset locally, the app keeps its solution-relative
dev defaults.

## Why no CORS

The browser only ever talks to the `web` container. nginx serves the site and
forwards `/api/*` to the gateway (stripping `/api`, mirroring the Vite dev
proxy), so every request is same-origin. The gateway has no CORS policy and
runs plain HTTP; TLS is terminated upstream by the NAS reverse proxy.

## Run it

```bash
docker compose build
docker compose up -d
```

The `web` container publishes host port `8088`. Point the NAS reverse proxy for
`omnomnom.compilesoftware.nl` at `http://<nas-host>:8088`.

Locally, browse to `http://localhost:8088/` to exercise the full catalogue and
checkout flow.

## Reseeding

Each endpoint recreates and seeds its database on startup. To reset to a clean
catalogue, wipe both volumes (the SQLite files and the in-flight messages
together, so no message references a row that no longer exists):

```bash
docker compose down
docker volume rm omnomnom_db omnomnom_transport
docker compose up -d
```

Volume names are prefixed with the compose project name (the repo folder,
`omnomnom`). Run `docker volume ls` to confirm the exact names on the host.
