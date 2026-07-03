# Build context is the src/ directory (project references span sibling
# projects). The AllInOne host references only *.Endpoint projects, so the
# plain runtime image (no ASP.NET) is enough.
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish OmNomNom.AllInOne/OmNomNom.AllInOne.csproj -c Release -o /app

FROM mcr.microsoft.com/dotnet/runtime:10.0
WORKDIR /app
COPY --from=build /app .
# Shared with the gateway so both processes use one transport folder and
# the same checkout.db. Mounted as volumes in docker-compose.
ENV OMNOMNOM_DB_DIR=/data/db \
    OMNOMNOM_TRANSPORT_DIR=/data/transport
ENTRYPOINT ["dotnet", "OmNomNom.AllInOne.dll"]
