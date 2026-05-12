# Migration plan: in-flight checkout state on WorkflowComposer

A staged rollout of WorkflowComposer across all four service boundaries that contribute to the in-flight checkout, with explicit notes on what's intentional, what's temporary, and what must change later.

## Why we're doing this

Before WorkflowComposer, in-flight checkout state lived in `IDistributedCache` with one cache key per service boundary (`UserOrder`, `FinanceOrder`, `ShippingOrders`, `CreditCard`). Every checkout page wrote a different cache key from a different handler; every page that needed to display existing data fell back to the cache. Several recurring symptoms came from this split:

- The cart row in `catalog.db` and the cart in the cache could disagree, especially after `SubmitOrderItems` had been dispatched but the handler hadn't run yet (race between async commit and synchronous read). Recent commits patched this with cache-then-DB fallbacks.
- A single HTTP request could write the cache successfully and then crash before sending the corresponding NServiceBus command, or the reverse. There was no atomicity between the cache and the bus.
- Read-your-writes was best-effort. The cache TTL is 30 minutes; cross-tab edits and back-button navigation could surface stale slices.

The brief considered three replacements (synchronous DB-then-publish, distributed transactions, NServiceBus TransactionalSession + outbox) and chose the third for per-boundary atomicity. After designing it out, we realized that genuinely cross-boundary atomicity isn't possible without 2PC, and that what we actually want is a *single* coordinator that owns the in-flight state and emits the per-boundary commands as one atomic outbox bundle at submit time.

That coordinator is WorkflowComposer. The per-boundary writes during checkout are plain SQLite writes against `checkout.db`; only the final submit goes through TransactionalSession.

## Architecture decisions worth preserving

These are the load-bearing choices. Don't undo them without re-deriving the conclusion.

- **Workflow state lives in IT/OPS, not in a service boundary.** Cart items, billing address, shipping address, delivery option, and payment token during checkout are workflow state, not domain state. Domain boundaries own their *committed* truth (catalog products, finance addresses, etc.); the workflow store owns *drafts*. Putting drafts in domain DBs would force per-boundary atomicity invariants on data the user hasn't actually committed to.
- **TransactionalSession is opened only inside `Submit`.** Per-page slice writes use a plain `SqliteConnection`. They don't publish events, so they don't need atomicity with the bus. The session has lifetime cost (control message round-trip on commit) and using it on every keystroke would be wasteful.
- **`SqliteWorkflowStore` is scoped, not singleton.** It depends on `ITransactionalSession`, which is scoped per-request and unusable after `Commit`.
- **Slice payloads are plain data, not domain entities.** The slice JSON is what gets serialized into `checkout.db`. Putting EF entities in there would couple the workflow store to a specific ORM and confuse "draft state" with "committed entity."
- **Submit commands are the existing per-boundary commands.** `SubmitOrderItems`, `SubmitBillingAddress`, etc. The brief required preserving message contracts; the slices' `BuildSubmitCommand` produces them. Don't invent a new `PlaceOrder` mega-command.
- **Each boundary's `IWorkflowSlice` is registered as singleton** (no per-request state; just metadata + `BuildSubmitCommand`).
- **`BuildSubmitCommand` returning `null` skips the slice.** Useful when a boundary's slice is optional or was never written for this workflow.

## Phases

### Phase 1 — Library, docs, Catalog pilot — DONE

Commits `3156e80`, `16d89ce` on `feature/workflowcomposer`.

- [x] `WorkflowComposer` core library: `IWorkflowStore`, `IWorkflowSlice`, `WorkflowSlice<T>`, `IWorkflowSubmitter`, DI helpers.
- [x] `WorkflowComposer.Sqlite` backend: `SqliteWorkflowStore` using NServiceBus TransactionalSession + outbox.
- [x] Docs: README, concepts, getting-started.
- [x] Catalog `CartWorkflowSlice` + record `CartSlice`.
- [x] All seven Catalog cart handlers migrated to `IWorkflowStore`.
- [x] `Catalog.ServiceComposition.Helpers.CacheHelper` deleted.
- [x] Gateway wired with `AddWorkflowComposer` + `UseSqliteStore` against `checkout.db`.

### Phase 2 — Finance slices — DONE

Commit `08e3b9d` on `feature/workflowcomposer`. The original plan called for one slice; Finance actually emits three commands (`SubmitOrderItems`, `SubmitBillingAddress`, `SubmitDeliveryOption`) so it needs three slices. The same per-command-per-slice rule applies to Phases 3 and 4.

- [x] `OrderItemsSlice` + `OrderItemsWorkflowSlice` (`SliceKey = "Finance.OrderItems"`) → `Finance.Endpoint.Messages.Commands.SubmitOrderItems`.
- [x] `BillingAddressSlice` + `BillingAddressWorkflowSlice` (`SliceKey = "Finance.BillingAddress"`) → `SubmitBillingAddress`.
- [x] `DeliveryOptionSlice` + `DeliveryOptionWorkflowSlice` (`SliceKey = "Finance.DeliveryOption"`) → `Finance.Endpoint.Messages.Commands.SubmitDeliveryOption`.
- [x] `WorkflowComposer` referenced from `Finance.ServiceComposition.csproj`.
- [x] All five Finance checkout handlers migrated to `IWorkflowStore`. `OrderSubmitHandler`, `AddressSubmitHandler`, `DeliveryOptionSubmitHandler` write their slices and continue to dispatch their existing commands directly (Phase 5 will move that). `AddressHandler` reads `BillingAddress` slice with the previous-order mock as fallback. `SummaryLoadedSubscriber` falls back to `OrderItems` slice instead of the cache.
- [x] `Finance.ServiceComposition.Helpers.CacheHelper.cs` deleted; `AddSingleton<CacheHelper>()` dropped from `Startup.cs`.
- [x] Three slices registered via `workflow.RegisterSlicesFromAssembliesOf(typeof(BillingAddressWorkflowSlice), ...)`.

### Phase 3 — Shipping slices — DONE

Commit `86d5537` on `feature/workflowcomposer`.

- [x] `ShippingAddressSlice` + `ShippingAddressWorkflowSlice` (`SliceKey = "Shipping.Address"`) → `SubmitShippingAddress`.
- [x] `DeliveryOptionSlice` + `DeliveryOptionWorkflowSlice` (`SliceKey = "Shipping.DeliveryOption"`) → `Shipping.Endpoint.Messages.Commands.SubmitDeliveryOption`.
- [x] `WorkflowComposer` referenced from `Shipping.ServiceComposition.csproj`.
- [x] Five Shipping checkout handlers migrated. `AddressSubmitHandler` and `DeliveryOptionSubmitHandler` write their slices and continue to dispatch their commands directly (Phase 5 will move that). `AddressHandler` reads the `ShippingAddress` slice; the previous-order mock is now only used when the slice is empty. `DeliveryOptionsHandler` and `SummaryHandler` read slice first with DB fallback.
- [x] `Shipping.ServiceComposition.Helpers.CacheHelper.cs` deleted; `AddSingleton<CacheHelper>()` dropped from `Startup.cs`.
- [x] Both slices registered via `workflow.RegisterSlicesFromAssembliesOf(typeof(ShippingAddressWorkflowSlice), ...)`.

### Phase 4 — PaymentInfo slice — DONE

Commit `a383cd8` on `feature/workflowcomposer`.

- [x] `PaymentSlice` + `PaymentWorkflowSlice` (`SliceKey = "PaymentInfo.Card"`) → `SubmitPaymentInfo`. Customer id stays hardcoded inside `BuildSubmitCommand` until real authentication is wired in.
- [x] `WorkflowComposer` referenced from `PaymentInfo.ServiceComposition.csproj`.
- [x] Three PaymentInfo checkout handlers migrated. `PaymentSubmitHandler` writes the slice and continues to dispatch `SubmitPaymentInfo` directly (Phase 5 will move that). `SummaryHandler` falls back to the slice instead of the cache. `PaymentHandler` (GET) now reads the slice first so returning to `/buy/payment` shows the just-selected card before `SubmitPaymentInfo` is processed.
- [x] `PaymentInfo.ServiceComposition.Helpers.CacheHelper.cs` deleted; `AddSingleton<CacheHelper>()` dropped from `Startup.cs`.
- [x] Slice registered via `workflow.RegisterSlicesFromAssembliesOf(..., typeof(PaymentWorkflowSlice))`.
- [x] **Bonus cleanup:** with no boundary using `IDistributedCache` anymore, `builder.Services.AddDistributedMemoryCache()` is gone from the gateway. All in-flight checkout state is now exclusively in `checkout.db`.

### Phase 5 — Wire `WorkflowSubmitter` and stand up the Checkout endpoint — DONE

Commits `99df629` and `d1ba237` on `feature/workflowcomposer`. The phase that flipped the system over to atomic submit.

- [x] `Checkout.Endpoint` exists under `src/Checkout.Endpoint/`. Tiny NServiceBus host with no business handlers, bound to `checkout.db` with `EnableOutbox()` and `EnableTransactionalSession()`. Added to the slnx (under `/ITOps/`) and to the `Website + Endpoints` JetBrains compound run config.
- [x] `WorkflowSubmitHandler` lives in `src/CompositionGateway/Checkout/`. POST `/buy/summary/{orderId}` writes the `CompleteOrder` marker slice and calls `IWorkflowSubmitter.Submit`. Going with the simpler in-gateway home for now; can split out into a `Checkout.ServiceComposition` project later if that folder ever gets siblings.
- [x] **`CompleteOrder` resolved as Option 1.** New `CompleteOrderWorkflowSlice` in `Catalog.ServiceComposition/Workflow/`. Marker slice (no payload) written by `WorkflowSubmitHandler` immediately before the submit call so `CompleteOrder` rides the same atomic dispatch bundle as the per-boundary commands. `Catalog.Endpoint` now configures three delayed retries (overriding the shared `NumberOfRetries(0)`) so `CompleteOrderHandler` can wait if it lands before `SubmitOrderItems` writes the Order row.
- [x] **All seven direct `IMessageSession.Send` calls removed** from the migrated submit handlers in Catalog/Finance/Shipping/PaymentInfo `ServiceComposition`. `SummarySubmitHandler.cs` is deleted - its only job was sending `CompleteOrder`, now part of the workflow.
- [x] **End-to-end smoke test passed.** Add cart, post addresses, post shipping option, post payment, POST `/buy/summary` → `submit HTTP 200`. Within seconds, all four boundary DBs have their `Order` row, Catalog's `InventoryDeltas` records the stock decrement (proving `CompleteOrderHandler` ran), and `checkout.db` has the `$submitted` marker.
- [x] **Failure-mode test passed.** Killed `Checkout.Endpoint` before submit, ran the full checkout flow, POST `/buy/summary` returned 200 with all four boundary DBs empty for the order. Restarted `Checkout.Endpoint`; within ~5s the outbox dispatched and all four boundaries received their commands. The atomic-or-nothing guarantee held end-to-end.

### Phase 6 — Cleanup

- [x] Remove `builder.Services.AddDistributedMemoryCache()` from `CompositionGateway/Program.cs` — done as part of Phase 4.
- [x] Confirm no `CacheHelper` exists in any `ServiceComposition` project — done after Phase 4.
- [ ] Remove any TODOs related to the cache-vs-DB consistency workarounds (see `git log --grep "Tolerate not-yet-processed"` etc.).
- [ ] Update `AGENTS.md` to describe the WorkflowComposer pattern as the canonical write-side mechanism.
- [ ] Consider whether `WorkflowComposer` and `WorkflowComposer.Sqlite` should be split out into their own repository at this point (NuGet, possibly into the ServiceComposer family). Out of scope for this rollout but worth a flag.

## Loose ends in the current state

After Phase 5 there are no temporary loose ends left. All seven direct `IMessageSession.Send` calls in `*.ServiceComposition` have been removed; `SummarySubmitHandler.cs` is deleted; `Checkout.Endpoint` exists as a real process; the workflow submit is the single dispatch path.

## Things deliberately not in scope

- **Saga orchestration.** WorkflowComposer's job ends when the bag of commands is enqueued. Sagas (e.g. `ShippingPolicy`) downstream stay where they are.
- **Cross-boundary partial-commit recovery.** If two service boundaries' NServiceBus message handlers act on the dispatched commands and one fails, that's a downstream messaging concern, not WorkflowComposer's. Handlers must be idempotent.
- **Multi-workflow-type support.** Today there's one workflow: checkout. The slice keys aren't namespaced per workflow. If a second workflow joins the system, revisit the key naming and the table layout.
- **Backend portability across SQLite-incompatible stores.** The current `IWorkflowStore.Submit` contract requires the backend to support an outbox-equivalent mechanism. A pure read/write store (e.g. Redis Hash) without atomic dispatch couldn't satisfy `Submit`. A Redis backend would need a complementary outbox table or stream.

## Phase 5 decisions (resolved)

- **Should `CompleteOrder` go through `WorkflowSubmitter`?** Yes. Implemented as `CompleteOrderWorkflowSlice`, a marker slice written by `WorkflowSubmitHandler` immediately before `Submit`. `Catalog.Endpoint` got delayed retries (3) so the receiver tolerates `CompleteOrder` arriving before `SubmitOrderItems` writes the Order row.
- **Where does `WorkflowSubmitHandler` live?** `CompositionGateway/Checkout/`. We didn't create a separate `Checkout.ServiceComposition` project; it would have only one file. If a second IT/OPS-level composition handler appears, that's the moment to extract.
- **What happens to the cart-page `OrderSubmitHandler` on `[HttpPost("/cart/{orderId}")]`?** After Phase 5 it just writes the cart slice. The frontend posts to it from the cart page; no other code relies on `SubmitOrderItems` being dispatched at that point. Smoke test confirmed.

## What success looks like at end of Phase 6

- One write-side library used identically by all four service boundaries.
- `checkout.db` is the single source of truth for in-flight workflow state.
- Exactly one `TransactionalSession` open per checkout, at submit time, against one DB.
- All four legacy `CacheHelper`s deleted; `IDistributedCache` deregistered.
- Failure semantics: a checkout submission either fully dispatches all six commands or none, with the workflow row's "submitted" flag committed atomically with the outbox.
- `OrderSubmitHandler`'s `messageSession.Send` is gone.
- The demo continues to teach UI Composition (read side, ServiceComposer) and now also Workflow Composition (write side, WorkflowComposer) as the two complementary IT/OPS frameworks that domain boundaries plug into.
