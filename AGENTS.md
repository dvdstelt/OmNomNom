# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## What This Project Is

OmNomNom is a **conference demo** for teaching UI Composition and service boundary patterns using NServiceBus. It simulates an e-commerce system decomposed into autonomous services that each own their data and communicate exclusively via messages.

## Commands

### Backend

```bash
# Build entire solution
dotnet build src/OmNomNom.sln

# Run an individual endpoint (repeat for each service)
dotnet run --project src/Catalog.Endpoint/Catalog.Endpoint.csproj
dotnet run --project src/Finance.Endpoint/Finance.Endpoint.csproj
dotnet run --project src/Shipping.Endpoint/Shipping.Endpoint.csproj
dotnet run --project src/PaymentInfo.Endpoint/PaymentInfo.Endpoint.csproj

# Run the API gateway
dotnet run --project src/CompositionGateway/CompositionGateway.csproj

# Run the back-office web app
dotnet run --project src/OmNomNom.BackOffice/OmNomNom.BackOffice.csproj
```

A JetBrains compound run configuration exists at `src/.run/Website + Endpoints.run.xml` that starts all services together.

There are no automated tests in this repository.

### Frontend

```bash
cd src/website
npm install
npm run dev   # Vite dev server on port 5173 (SvelteKit)
npm run build
```

## Architecture

### Service Decomposition

Each business domain (Catalog, Finance, Shipping, PaymentInfo) is split into three projects:

| Project suffix | Purpose |
|---|---|
| `.Endpoint` | NServiceBus message handlers and business logic; owns its LiteDB database |
| `.ServiceComposition` | Contributes data to HTTP responses via ServiceComposer (no database access) |
| `.Endpoint.Messages` | Shared command/event contracts consumed by other services |
| `.ServiceComposition.Events` | Composition events raised between service composers |

### UI Composition via ServiceComposer

The `CompositionGateway` does **not** aggregate data itself. Instead, each service's `ServiceComposition` project registers `ICompositionRequestsHandler` implementations that respond to HTTP routes. When a request hits the gateway, ServiceComposer fans it out to all registered handlers; each service appends its own fields to the JSON response. This is the central pattern being demonstrated.

A service composer can raise `IPublishEvent` to notify sibling composers (e.g. Catalog raises `SummaryLoaded` so Finance can append pricing data). These composition events are **in-process only** and defined in `.ServiceComposition.Events` projects.

### Async Messaging via NServiceBus

- **Transport:** `LearningTransport` (file-based, development only). Messages are written to `.learningtransport/` folders on disk - no broker required.
- **Persistence:** `LearningPersistence` for saga state.
- **Routing convention:** type namespace determines message kind:
  - `*.Messages.Commands` -> command (sent to one endpoint)
  - `*.Messages.Events` -> event (published to subscribers)
- **Saga:** `ShippingPolicy` in `Shipping.Endpoint` coordinates shipping by waiting for both `OrderAccepted` and `PaymentSucceeded` before dispatching `ShipOrderRequest`.

### Data

Each endpoint uses its own embedded **LiteDB** database configured via `appsettings.json`. Services never share a database. Schema and seed data are applied by each service's `DatabaseInitializer` on startup.

### Frontend

SvelteKit SPA (`src/website`, `@sveltejs/adapter-static`) calls the `CompositionGateway` directly. CORS is configured in the gateway for `localhost:5173`.

The frontend mirrors the backend's service-boundary decomposition: each top-level folder under `src/website/src/` other than `lib/`, `routes/`, and `static/` is owned by one service boundary (`Branding`, `Catalog`, `Marketing`, `Finance`, `Shipping`, `PaymentInfo`). The components inside those folders are **microviews** — small `.svelte` files that consume only the JSON slice their service contributed to the composed gateway response. Microviews never fetch; the Branding page that owns the route makes one `gateway.*` call per navigation and passes slices down as props.

Branding owns site chrome (layout, header, footer, cart indicator, filter bar, checkout progress bar) and the `routes/` files that compose microviews into pages. The global stylesheet is `src/Branding/styles/global.css` (ported from `html/css/styles.css`).
