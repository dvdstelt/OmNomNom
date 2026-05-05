# OmNomNom website (SvelteKit SPA)

The frontend host for the OmNomNom demo. SvelteKit 2 with `adapter-static`,
served as a pure client-side SPA from `https://localhost:5173` against the
CompositionGateway on `https://localhost:7126`.

## Run

```bash
npm install
npm run dev      # vite dev server, port 5173 (HTTPS)
npm run build    # static SPA into ./build
npm run preview  # serve the built SPA
```

The Rider/VS run config in `src/OmNomNom.slnLaunch` (`Website + Endpoints`)
launches `npm run dev` for this project alongside the .NET endpoints.

## Service-folder convention

Each top-level folder under `src/` other than `lib/`, `routes/`, and
`static/` represents a service boundary that owns its UI:

- `Branding/` — site chrome: layout, the global stylesheet ported from
  `/html/css/styles.css`, header (regular + checkout), footer, cart
  indicator, filter bar, checkout progress bar. Branding's pages live in
  `routes/` and orchestrate composition.
- `Catalog/`, `Marketing/`, `Finance/`, `Shipping/`, `PaymentInfo/` —
  each folder mirrors the matching `*.ServiceComposition` .NET project
  on the backend. The components inside are **microviews**: small,
  presentational `.svelte` files that consume the JSON slice their
  service contributed to the composed gateway response.

Microviews **never** call the API. They take their slice as props.
Branding pages do exactly one `gateway.*` call per route and pass slices
down; this mirrors what the CompositionGateway already does on the
server (one route, fan-out to per-service composers, one composed JSON
response).

## Adding a new field

1. Add the field server-side in the relevant `*.ServiceComposition`
   project's composer or subscriber.
2. Add or update the matching microview under the corresponding folder
   here. No other folder needs to change.

## Where things live

- `routes/+layout.svelte` — wraps every page in `Branding/Layout`,
  imports the global stylesheet, seeds the cart count store from the
  persisted orderId.
- `routes/+layout.js` — disables SSR/prerender (pure client SPA).
- `lib/api/gateway.js` — single client for all CompositionGateway calls.
- `lib/stores/orderId.js` — persistent orderId in localStorage.
- `lib/stores/cart.js` — cart-count store driven by the gateway's
  `/cart/{orderId}` response.
- `static/products/*.png` — product images served at `/products/<file>`.
- `static/img/cards/*.png` — credit card brand icons.
