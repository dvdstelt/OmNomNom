# Single-image build of the whole OmNomNom demo: the AllInOne endpoint host,
# the CompositionGateway, and nginx (serving the SvelteKit static site and
# proxying /api to the gateway) all run in one container under supervisord.
# Everything is same-origin, so no CORS; persistence is one /data volume.

# 1. Build the SvelteKit static site.
FROM node:22-alpine AS web
WORKDIR /web
COPY src/website/package.json src/website/package-lock.json ./
RUN npm ci
COPY src/website/ ./
RUN npm run build

# 2. Publish both .NET apps. Context is the repo root; src/ holds all the
#    cross-referenced projects.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS dotnet
WORKDIR /src
COPY src/ ./
RUN dotnet publish OmNomNom.AllInOne/OmNomNom.AllInOne.csproj -c Release -o /app/allinone \
 && dotnet publish CompositionGateway/CompositionGateway.csproj -c Release -o /app/gateway

# 3. Runtime: ASP.NET base (its shared framework also runs the AllInOne
#    console host) plus nginx and supervisord.
FROM mcr.microsoft.com/dotnet/aspnet:10.0
RUN apt-get update \
 && apt-get install -y --no-install-recommends nginx supervisor curl \
 && rm -rf /var/lib/apt/lists/*
COPY --from=dotnet /app /app
COPY --from=web /web/build /usr/share/nginx/html
COPY deploy/nginx.conf /etc/nginx/sites-available/default
COPY deploy/supervisord.conf /etc/supervisor/conf.d/omnomnom.conf
EXPOSE 80

# Health check: the AllInOne host's liveness probe (/health/live on :8081,
# backed by NServiceBusContrib.HealthCheck), which aggregates every endpoint
# in the process. "unhealthy" means an endpoint has died, not merely that
# it's still warming up (warm-up shows as not-ready, not not-alive), so a
# failure here is a genuine reason to restart. The start period covers .NET
# cold start and on-boot database seeding before failures count.
#
# NOTE: this only marks the container unhealthy. Restarting on unhealthy needs
# the autoheal sidecar in docker-compose.yml (Docker won't do it on its own).
HEALTHCHECK --interval=30s --timeout=5s --start-period=90s --retries=3 \
  CMD curl -fsS -o /dev/null http://localhost:8081/health/live || exit 1

CMD ["/usr/bin/supervisord", "-c", "/etc/supervisor/supervisord.conf"]
