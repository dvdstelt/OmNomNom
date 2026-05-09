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

### Phase 5 — Wire `WorkflowSubmitter` and stand up the Checkout endpoint

This is the phase that flips the system over to atomic submit. Until this phase lands, the Phase 1 commit's `OrderSubmitHandler` is still dispatching `SubmitOrderItems` directly via `IMessageSession`. Same is true for whatever direct `Send`s exist in Finance/Shipping/PaymentInfo `OrderSubmitHandler` analogues.

- [ ] Create a `Checkout.Endpoint` project: a tiny NServiceBus host with no message handlers of its own. Configured with the SQLite persister against `checkout.db`, `EnableOutbox()`, `EnableTransactionalSession()` (no `ProcessorEndpoint` — it *is* the processor endpoint). Add to slnx, add to the JetBrains run configuration.
- [ ] Add a `WorkflowSubmitHandler` (in `CompositionGateway` or in a new `Checkout.ServiceComposition` project under IT/OPS) for `[HttpPost("/buy/summary/{orderId}")]` that calls `submitter.Submit(orderId, ct)`. This is the single user-facing trigger for the atomic fan-out.
- [ ] Decide where `CompleteOrder` lives. Today it's sent by `Catalog.ServiceComposition.Checkout.SummarySubmitHandler` via `IMessageSession`. Two options:
   - Add a `CompleteOrderSlice` whose `BuildSubmitCommand` returns `CompleteOrder` unconditionally, so it goes out as part of the atomic bundle.
   - Keep `SummarySubmitHandler` separate, sending `CompleteOrder` only after the workflow submit returns successfully.

   The first option is cleaner (everything atomic) but requires `CompleteOrder` to tolerate arrival before/concurrent with `SubmitOrderItems` etc. — verify the receiver-side handler is idempotent and order-independent. Default to option 1 unless evidence says otherwise.
- [ ] **Remove the direct `IMessageSession.Send` from each per-boundary `OrderSubmitHandler` analogue.** Specifically:
   - `Catalog.ServiceComposition.Checkout.OrderSubmitHandler.cs` — drop `messageSession.Send(message)`. The handler becomes a slice-write only (or fold into the cart-page POST handling).
   - The same in Finance/Shipping/PaymentInfo if they have analogous handlers that call `Send` directly during checkout.
- [ ] Smoke-test the full checkout end-to-end: add cart items, post addresses, post payment, hit `/buy/summary` POST, verify all six commands land in their respective endpoints and that `checkout.db` records the workflow as submitted with the outbox row dispatched.
- [ ] Failure-mode tests:
   - Kill `Checkout.Endpoint` between session commit and dispatch — restart it and confirm outbox replays.
   - Kill the gateway mid-commit — confirm no outbox row, no slice changes, and the user can retry.

### Phase 6 — Cleanup

- [x] Remove `builder.Services.AddDistributedMemoryCache()` from `CompositionGateway/Program.cs` — done as part of Phase 4.
- [x] Confirm no `CacheHelper` exists in any `ServiceComposition` project — done after Phase 4.
- [ ] Remove any TODOs related to the cache-vs-DB consistency workarounds (see `git log --grep "Tolerate not-yet-processed"` etc.).
- [ ] Update `AGENTS.md` to describe the WorkflowComposer pattern as the canonical write-side mechanism.
- [ ] Consider whether `WorkflowComposer` and `WorkflowComposer.Sqlite` should be split out into their own repository at this point (NuGet, possibly into the ServiceComposer family). Out of scope for this rollout but worth a flag.

## Loose ends in the current state — these are temporary, do not normalize them

The repo is intentionally in a half-migrated state. Don't paper over these — they're scheduled to be removed in later phases.

| Where | What | When it goes away |
|---|---|---|
| [`Catalog.ServiceComposition/Checkout/OrderSubmitHandler.cs`](../../src/Catalog.ServiceComposition/Checkout/OrderSubmitHandler.cs) | Still calls `messageSession.Send(new SubmitOrderItems {...})` after writing the slice | Phase 5 |
| [`Catalog.ServiceComposition/Checkout/SummarySubmitHandler.cs`](../../src/Catalog.ServiceComposition/Checkout/SummarySubmitHandler.cs) | Still calls `messageSession.Send(new CompleteOrder {...})` directly | Phase 5 |
| [`Finance.ServiceComposition/Checkout/OrderSubmitHandler.cs`](../../src/Finance.ServiceComposition/Checkout/OrderSubmitHandler.cs) | Still dispatches Finance's `SubmitOrderItems` after writing the slice | Phase 5 |
| [`Finance.ServiceComposition/Checkout/AddressSubmitHandler.cs`](../../src/Finance.ServiceComposition/Checkout/AddressSubmitHandler.cs) | Still dispatches `SubmitBillingAddress` after writing the slice | Phase 5 |
| [`Finance.ServiceComposition/Checkout/DeliveryOptionSubmitHandler.cs`](../../src/Finance.ServiceComposition/Checkout/DeliveryOptionSubmitHandler.cs) | Still dispatches Finance's `SubmitDeliveryOption` after writing the slice | Phase 5 |
| [`Shipping.ServiceComposition/Checkout/AddressSubmitHandler.cs`](../../src/Shipping.ServiceComposition/Checkout/AddressSubmitHandler.cs) | Still dispatches `SubmitShippingAddress` after writing the slice | Phase 5 |
| [`Shipping.ServiceComposition/Checkout/DeliveryOptionSubmitHandler.cs`](../../src/Shipping.ServiceComposition/Checkout/DeliveryOptionSubmitHandler.cs) | Still dispatches Shipping's `SubmitDeliveryOption` after writing the slice | Phase 5 |
| [`PaymentInfo.ServiceComposition/Checkout/PaymentSubmitHandler.cs`](../../src/PaymentInfo.ServiceComposition/Checkout/PaymentSubmitHandler.cs) | Still dispatches `SubmitPaymentInfo` after writing the slice | Phase 5 |
| [`CompositionGateway/Program.cs`](../../src/CompositionGateway/Program.cs) | `ProcessorEndpoint = "Checkout"` references an endpoint that doesn't exist as a process | Phase 5 (when Checkout.Endpoint is created) |

The "Checkout endpoint doesn't exist yet" loose end is benign as long as nothing calls `session.Commit()`. The current phases only use slice reads and writes, so the session is never opened. As soon as Phase 5 wires `WorkflowSubmitHandler`, the Checkout endpoint must exist before any user clicks Submit.

## Things deliberately not in scope

- **Saga orchestration.** WorkflowComposer's job ends when the bag of commands is enqueued. Sagas (e.g. `ShippingPolicy`) downstream stay where they are.
- **Cross-boundary partial-commit recovery.** If two service boundaries' NServiceBus message handlers act on the dispatched commands and one fails, that's a downstream messaging concern, not WorkflowComposer's. Handlers must be idempotent.
- **Multi-workflow-type support.** Today there's one workflow: checkout. The slice keys aren't namespaced per workflow. If a second workflow joins the system, revisit the key naming and the table layout.
- **Backend portability across SQLite-incompatible stores.** The current `IWorkflowStore.Submit` contract requires the backend to support an outbox-equivalent mechanism. A pure read/write store (e.g. Redis Hash) without atomic dispatch couldn't satisfy `Submit`. A Redis backend would need a complementary outbox table or stream.

## Open questions to resolve in Phase 5

- **Should `CompleteOrder` go through `WorkflowSubmitter`?** See Phase 5 step 3. Default to yes; confirm receiver idempotence.
- **Where does `WorkflowSubmitHandler` live?** In `CompositionGateway/` directly, or in a new `Checkout.ServiceComposition` project under IT/OPS that mirrors the per-boundary `*.ServiceComposition` projects? The latter is more consistent with the existing folder convention.
- **What happens to the existing `OrderSubmitHandler` on `[HttpPost("/cart/{orderId}")]`?** That route is the cart-page submit (not the workflow submit at `/buy/summary`). After Phase 5 it just writes the cart slice — no message. Confirm the frontend still posts to it, and that nothing relies on `SubmitOrderItems` being dispatched at that point in the flow.

## What success looks like at end of Phase 6

- One write-side library used identically by all four service boundaries.
- `checkout.db` is the single source of truth for in-flight workflow state.
- Exactly one `TransactionalSession` open per checkout, at submit time, against one DB.
- All four legacy `CacheHelper`s deleted; `IDistributedCache` deregistered.
- Failure semantics: a checkout submission either fully dispatches all six commands or none, with the workflow row's "submitted" flag committed atomically with the outbox.
- `OrderSubmitHandler`'s `messageSession.Send` is gone.
- The demo continues to teach UI Composition (read side, ServiceComposer) and now also Workflow Composition (write side, WorkflowComposer) as the two complementary IT/OPS frameworks that domain boundaries plug into.
