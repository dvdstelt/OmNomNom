# Build context is the src/ directory (project references span sibling
# projects).
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish CompositionGateway/CompositionGateway.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build /app .
# Production disables the dev-only HTTPS redirect; the gateway serves plain
# HTTP behind the nginx/NAS reverse proxy. Storage paths are shared with
# the AllInOne host via docker-compose volumes.
ENV ASPNETCORE_URLS=http://+:8080 \
    ASPNETCORE_ENVIRONMENT=Production \
    OMNOMNOM_DB_DIR=/data/db \
    OMNOMNOM_TRANSPORT_DIR=/data/transport
EXPOSE 8080
ENTRYPOINT ["dotnet", "CompositionGateway.dll"]
