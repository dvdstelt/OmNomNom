# OmNomNom: blog series outline

A reading order and writer's reference for a multi-part series on the architecture demonstrated in this repository. Each section below names a post, sketches the angle, lists the code paths to walk readers through, and links to the existing docs that already cover the territory in depth.

The series builds bottom-up: a reader who only finishes the first three posts has the conceptual model; later posts deepen specific subsystems and finish with the NServiceBus 10.2 features adopted in [PR #171](https://github.com/dvdstelt/OmNomNom/pull/171).

---

## Series at a glance

| # | Post | Anchor concept | Existing docs |
|---|---|---|---|
| 1 | Why service boundaries | A boundary is a *decision*, not a deployment | — |
| 2 | OmNomNom's decomposition | Six message boundaries + two web hosts | [boundaries.md](boundaries.md) |
| 3 | Data ownership without sharing | Same `Product` row, modelled five different ways | [boundaries.md](boundaries.md) |
| 4 | Read-side UI composition (ServiceComposer) | One HTTP request fans out, every boundary contributes its slice of the JSON | — |
| 5 | The SvelteKit microview pattern | The frontend mirrors the backend boundaries: one folder per service | — |
| 6 | Write-side workflow composition (WorkflowComposer) | In-flight checkout state belongs to IT/OPS, not to any domain | [workflowcomposer/](workflowcomposer/README.md) |
| 7 | Atomic submit: TransactionalSession + outbox | One SQLite file, one transaction, six commands dispatched together | [workflowcomposer/concepts.md](workflowcomposer/concepts.md) |
| 8 | NServiceBus the messaging story | Commands, events, the saga that waits for two preconditions | — |
| 9 | NServiceBus 10.2: convention-based handlers | `[Handler]`, source-generated registration, the interceptor reveal | this doc, [PR #171](https://github.com/dvdstelt/OmNomNom/pull/171) |
| 10 | NServiceBus 10.2: hosting all endpoints in one process | `AddNServiceBusEndpoint`, the topology toggle in Rider | this doc, [PR #171](https://github.com/dvdstelt/OmNomNom/pull/171) |
| 11 | Boundary != process (the demo's payoff) | Same code, two deployment shapes, picked by which Rider compound runs | this doc |

Posts 1-3 are the conceptual foundation. 4-5 are the read side. 6-7 are the write side. 8-11 are the messaging plumbing and the new NSB 10.2 surface that ties them together.

---

## 1. Why service boundaries

**Angle.** A service boundary is a unit of *decision-making authority over a piece of business behaviour*. It is not "a microservice" and not "a database". The post argues that decomposing a system by domain capability beats decomposing by data shape, deployment, or team.

**Talking points.**
- The CRUD-monolith failure mode: every screen reads from every table, every team writes to every table, every change needs three teams.
- The "microservice" failure mode: split by entity (Customer, Order, Product) and you've sharded the CRUD database across processes without splitting the *decisions*.
- A boundary owns a set of business decisions about a slice of data. Different boundaries can hold different fields of the same conceptual entity. The "single source of truth" lives inside the boundary that decides about that field.
- Boundaries communicate *only* via messages (commands and events), never by reading each other's databases. Composition of *views* (UI composition) is a separate concern from communication of *intent* (messages).

**Pull-quotes from this codebase.**
- Catalog owns `Product.Name`. Finance owns `Product.Price`. Marketing owns `Product.OrderCount`. **Nobody owns "Product"** ([boundaries.md](boundaries.md)).
- Catalog and Marketing both hold a `Product` row keyed by the same `ProductId`. They each store *only* the fields they decide about, and they each receive a Catalog-published `ProductPublished` (or seed) to learn that the ID exists.

**Suggested reading order.** Pair this post with Udi Dahan's "Finding Service Boundaries", and Mauro Servienti's "All our aggregates are wrong" talks if linking out.

---

## 2. OmNomNom's decomposition

**Angle.** A walkthrough of the actual moving parts: six message boundaries, plus two ASP.NET hosts that are *not* boundaries (the API gateway and the back-office UI) and the SvelteKit storefront. The post answers "what are all these projects?"

**Talking points.**
- One business capability per boundary: Catalog (catalogue + inventory), Finance (pricing + billing), Marketing (popularity + email teasers), Shipping (logistics + saga), PaymentInfo (credit cards + fraud), Checkout (transactional-session processor).
- Each boundary is split into three projects: `<X>.Endpoint` (handlers + own SQLite), `<X>.ServiceComposition` (HTTP composers, no DB access), `<X>.Endpoint.Messages` (shared command/event contracts). Some also have `<X>.ServiceComposition.Events` for in-process composition events.
- The two non-boundary hosts: `CompositionGateway` (HTTP front door; runs ServiceComposer + WorkflowComposer; has a send-only NServiceBus endpoint for transactional session) and `OmNomNom.BackOffice` (email sender; embeds an NSB endpoint).
- The SvelteKit `website` calls only the gateway. It never talks to any boundary directly.

**Diagram.** A two-tier diagram: top row "deployment hosts" (Website → Gateway → 6 endpoints, plus BackOffice as a sibling); bottom row "logical boundaries" (Catalog, Finance, etc. drawn vertically through the hosts that contribute their slice). The point: deployment hosts and logical boundaries are *different planes*.

**Code anchors.**
- [src/OmNomNom.slnx](../src/OmNomNom.slnx) for the full project list grouped by boundary folder.
- [AGENTS.md](../AGENTS.md) "Architecture" section for the project naming convention.

---

## 3. Data ownership without sharing

**Angle.** Drill into how the same conceptual entity ends up modelled five different ways. The point is that *nobody shares a database* and *nobody shares an entity definition* — and that's the feature, not the bug.

**Talking points.**
- The `Product` entity exists in [Catalog.Data/Models](../src/Catalog.Data/Models), [Finance.Data/Models](../src/Finance.Data/Models), and [Marketing.Data/Models](../src/Marketing.Data/Models). Same `ProductId`. Different fields. Three separate SQLite files.
- The same is true for `Order`: Catalog tracks the items and fulfilment, Finance tracks the billing total and shipping option, Shipping tracks the delivery address.
- Each boundary's `<X>.Data` project is a *private* model. Other boundaries cannot see it. Cross-boundary contracts are in `<X>.Endpoint.Messages` (the public shape) and `<X>.ServiceComposition.Events` (the in-process composition shape).
- The price-leak example: the post can use the existing [docs/plan.md](plan.md) (the pricing-boundary fix) as a real-world illustration of how a boundary violation is detected and fixed.

**Existing material.** [boundaries.md](boundaries.md) has the full ownership table. The blog post is essentially a guided tour of that table with screenshots of the relevant `*.Data/Models` classes side-by-side.

---

## 4. Read-side UI composition (ServiceComposer)

**Angle.** How a single HTTP request to `GET /cart/{orderId}` is composed from contributions by Catalog (which products are in the cart), Finance (their current prices), and possibly Marketing (any trending tags). The gateway aggregates *nothing itself* — it's a fan-out coordinator.

**Talking points.**
- The gateway registers `ICompositionRequestsHandler` implementations from every `<X>.ServiceComposition` project. When a request hits a registered route, ServiceComposer dispatches it to *all* matching handlers in parallel, then merges their JSON contributions.
- Each composer is a one-screen class: it gets `request.GetComposedResponseModel()` (a dynamic dictionary), appends its boundary's fields, and returns. Order of contributions doesn't matter.
- Cross-composer coordination is via `IPublishEvent`. Example: Catalog publishes a `SummaryLoaded` composition event so Finance can append pricing data for the order's line items without Finance having to query its own DB twice. These events are *in-process only* and live in `<X>.ServiceComposition.Events`.
- The frontend trusts the gateway response shape. It doesn't know how many boundaries contributed — only that the JSON it asked for is there.

**Code anchors.**
- [src/CompositionGateway/Program.cs](../src/CompositionGateway/Program.cs) for the wiring.
- [src/Catalog.ServiceComposition/Cart/ShoppingCartComposer.cs](../src/Catalog.ServiceComposition/Cart/ShoppingCartComposer.cs) for an example composer that reads the workflow store + queries its own DB.
- [src/Catalog.ServiceComposition/Products/OnSummaryLoaded.cs](../src/Catalog.ServiceComposition/Products/OnSummaryLoaded.cs) for an `IPublishEvent` consumer.

---

## 5. The SvelteKit microview pattern

**Angle.** The frontend mirrors the backend's boundaries. Each top-level folder under `src/website/src/` (other than `lib/`, `routes/`, `static/`) is owned by one service boundary: `Catalog/`, `Finance/`, `Marketing/`, `Shipping/`, `PaymentInfo/`, `Branding/`. The components inside are *microviews* — small Svelte files that consume only the JSON slice their service contributed to the composed response.

**Talking points.**
- Microviews **never fetch**. The route file in `routes/` makes one `gateway.*` call per navigation; the microview gets its slice as a prop.
- `Branding/` owns the site chrome (header, footer, cart indicator, filter bar, checkout progress bar) and the route files. It's the only "service boundary" that knows about routing.
- The boundary alignment means a frontend change for, say, the Shipping address form lives next to the backend's `Shipping.Endpoint.Messages.SubmitShippingAddress` contract. Reviewers see both sides of the boundary in one diff.

**Code anchors.**
- [src/website/src/](../src/website/src/) folder listing.
- [AGENTS.md](../AGENTS.md) "Frontend" section.
- One of the route files in [src/website/src/routes/](../src/website/src/routes/) for the fetch-once-pass-down pattern.

---

## 6. Write-side workflow composition (WorkflowComposer)

**Angle.** The companion to ServiceComposer for the *write* side. ServiceComposer fans an HTTP read out across boundaries; WorkflowComposer lets each boundary contribute a slice of in-flight state (the customer's choices during a multi-page checkout) to a shared store, then submits everything atomically at the end.

**Talking points.**
- The problem: a checkout has cart items, billing address, shipping address, delivery option, payment token. Without a coordinator, each boundary stores its slice in its own DB before the customer has actually committed — leaving "draft" rows everywhere.
- WorkflowComposer keeps in-flight state in *one* place, owned by IT/OPS (not by any domain boundary). Domain boundaries see only committed truth.
- Each boundary implements `IWorkflowSlice<TSlice>` — a typed payload (e.g. `CartSlice`, `BillingAddressSlice`), a `SliceKey`, a `Validate` method, and a `BuildSubmitCommand` that translates the slice into the command sent at submit time.
- Slices are auto-discovered: see [src/WorkflowComposer/ServiceCollectionExtensions.cs](../src/WorkflowComposer/ServiceCollectionExtensions.cs) and `DiscoverSlices()` in the gateway's `Program.cs`.

**Code anchors.**
- [src/WorkflowComposer/IWorkflowSlice.cs](../src/WorkflowComposer/IWorkflowSlice.cs) for the interface.
- [src/Catalog.ServiceComposition/Workflow/CartWorkflowSlice.cs](../src/Catalog.ServiceComposition/Workflow/CartWorkflowSlice.cs) and [CompleteOrderWorkflowSlice.cs](../src/Catalog.ServiceComposition/Workflow/CompleteOrderWorkflowSlice.cs) for two slice examples.
- [docs/workflowcomposer/README.md](workflowcomposer/README.md) is already a thorough writeup — the blog post is the "trailer" for it.

---

## 7. Atomic submit: TransactionalSession + outbox

**Angle.** The most subtle moment in the demo: when the customer clicks "Place order", six different boundary endpoints each need a command. If two of those land and four don't, the customer is half-billed, half-shipped. WorkflowComposer + NServiceBus TransactionalSession make the dispatch atomic against the workflow's SQLite file.

**Talking points.**
- The gateway is a *send-only* NServiceBus endpoint. It opens a transactional session against `checkout.db`. Inside the session, it asks WorkflowComposer to build all the per-boundary commands, then commits.
- On commit, NServiceBus writes the outgoing messages to the outbox table in the same SQLite file as the workflow's slices. The transaction either commits everything or nothing.
- The `Checkout.Endpoint` is *the processor* for that outbox. It dispatches the queued messages out to the real endpoints (Catalog, Finance, Shipping, PaymentInfo). The boundary endpoints don't know the gateway used a transactional session — they just receive their command.
- The post is a great spot to explain why Checkout endpoint has *no business handlers* in [src/Checkout.Endpoint/](../src/Checkout.Endpoint/): its sole job is to be the outbox-dispatch worker.

**Existing material.** [docs/workflowcomposer/concepts.md](workflowcomposer/concepts.md) is the deep-dive reference.

---

## 8. NServiceBus messaging — commands, events, sagas

**Angle.** The actual choreography of an order placement, end to end. The post follows one `CompleteOrder` command from the gateway's transactional commit all the way to the customer receiving an email.

**Talking points.**
- **Commands** go to one endpoint (`CompleteOrder` → Catalog). **Events** are published, every subscriber gets a copy (`OrderPlaced` → Finance + Marketing + Shipping).
- The routing convention is namespace-based ([EndpointConfigurationExtensions.cs](../src/ITOps.Shared/EndpointConfiguration/EndpointConfigurationExtensions.cs)): types in `*.Messages.Commands` are commands, `*.Messages.Events` are events. No attribute decoration on the message classes.
- The saga: [`ShippingPolicy`](../src/Shipping.Endpoint/Sagas/ShippingPolicy.cs) is started by *either* `OrderAccepted` (Catalog confirms inventory) *or* `PaymentSucceeded` (Finance confirms the charge), whichever arrives first. Once *both* arrive, it sends `ShipOrderRequest` and waits for `ShipOrderReply`. On reply, it publishes `OrderShipped`. BackOffice subscribes to `OrderShipped` and sends the customer their confirmation email.
- The post can be a single end-to-end sequence diagram with the saga as the visible coordinator.

**Code anchors.**
- All `*.Endpoint.Messages` projects for the message contracts.
- [src/Shipping.Endpoint/Sagas/ShippingPolicy.cs](../src/Shipping.Endpoint/Sagas/ShippingPolicy.cs) for the saga itself.
- [src/Catalog.Endpoint/Handlers/CompleteOrderHandler.cs](../src/Catalog.Endpoint/Handlers/CompleteOrderHandler.cs) — the entry point handler.

---

## 9. NServiceBus 10.2: convention-based handlers

**Angle.** The new shape handlers can take in NServiceBus 10.2. Show the before, show the after, then open the source generator's output and explain the interceptor trick.

**Talking points.**
- Before 10.2: `public class CompleteOrderHandler : IHandleMessages<CompleteOrder>` — the interface declares which messages the class handles, and NSB's reflection-based scanner finds it.
- After 10.2 (PR #7641): `[Handler] public class CompleteOrderHandler` — no interface. The class has a `public Task Handle(CompleteOrder message, IMessageHandlerContext context)` method and that's enough. Registration is explicit: `endpointConfiguration.AddHandler<CompleteOrderHandler>()`.
- The reveal: the analyzer intercepts the `AddHandler<T>()` call site at compile time (C# 12 interceptors) and *generates an adapter class* that does implement `IHandleMessages<T>` and forwards `Handle` to the user's class. The runtime is unchanged.
- Walk through `obj/generated/.../InterceptionsOfAddHandlerMethod.g.cs` for Catalog. The `[InterceptsLocationAttribute]` even points back at the exact line in `Program.cs` (via `CatalogEndpointHostingExtensions`).
- Sagas keep their interfaces (`[Saga] : Saga<TData>, IAmStartedByMessages<T>`). The interfaces carry semantic meaning the runtime needs — "this message starts the saga" — that no method-shape convention currently captures. The post should say this explicitly so readers don't try and fail to "convention-ify" their sagas.

**Code anchors.**
- The before/after diff: any of the migration commits on [PR #171](https://github.com/dvdstelt/OmNomNom/pull/171), e.g. the `831374f` commit that dropped `IHandleMessages<T>` across all 13 non-saga handlers.
- The generated adapter: `src/Catalog.Endpoint/obj/generated/NServiceBus.Core.Analyzer/.../InterceptionsOfAddHandlerMethod.g.cs` (visible because [Catalog.Endpoint.csproj](../src/Catalog.Endpoint/Catalog.Endpoint.csproj) sets `EmitCompilerGeneratedFiles=true`).
- [Source PR upstream: NServiceBus#7641](https://github.com/Particular/NServiceBus/pull/7641).

---

## 10. NServiceBus 10.2: hosting all endpoints in one process

**Angle.** The other headline feature of 10.2: `IServiceCollection.AddNServiceBusEndpoint(...)`. Before, you had `hostBuilder.UseNServiceBus(cfg)` from `NServiceBus.Extensions.Hosting`; that package is gone in OmNomNom now. With the new API, you can register *multiple* endpoint configurations in one service collection, each with a keyed identifier, and the host runs them all with proper DI isolation.

**Talking points.**
- The new registration surface: `services.AddNServiceBusEndpoint(endpointConfiguration, "Catalog")`. In single-endpoint hosts the identifier is optional; in multi-endpoint hosts it's required (the runtime fails fast with a clear exception if you forget — discovered the hard way in the spike).
- Invariants when you go multi-endpoint: assembly scanning must be disabled on each config (so the configs don't pick up each other's `[Handler]` types from the shared load context), each endpoint needs a distinct transport instance, and the keyed-DI scopes keep `IMessageSession` resolutions per-endpoint.
- The idiomatic shape (after Daniel Marbach's review on [PR #171](https://github.com/dvdstelt/OmNomNom/pull/171)): each boundary exposes a `services.AddXEndpoint()` extension method. Standalone `Program.cs` is six lines. AllInOne is a fluent chain. Database seeding moves into an `IHostedLifecycleService` so `StartingAsync` runs the EF Core `EnsureCreatedAsync` before any message pump starts.
- The post can close with the `src/Directory.Build.props` trick to opt every project into NSB's V2 deterministic host-ID (XxHash128 replaces the deprecated MD5).

**Code anchors.**
- [src/OmNomNom.AllInOne/Program.cs](../src/OmNomNom.AllInOne/Program.cs) — the fluent chain in eight lines.
- Any `<X>EndpointHostingExtensions.cs` — e.g. [Catalog](../src/Catalog.Endpoint/CatalogEndpointHostingExtensions.cs).
- [src/ITOps.Shared/EndpointConfiguration/DatabaseSeederHostedService.cs](../src/ITOps.Shared/EndpointConfiguration/DatabaseSeederHostedService.cs) — the shared seeder base.
- [src/Directory.Build.props](../src/Directory.Build.props) — the host-ID switch.
- [Source PR upstream: NServiceBus#7633](https://github.com/Particular/NServiceBus/pull/7633).

---

## 11. Boundary != process (the demo's payoff)

**Angle.** The big reveal that the talk builds toward. Same boundaries, same handlers, same SQLite files; *different deployment topology* depending on which Rider compound you launch.

**Talking points.**
- Compound A — `Website + Endpoints`: 6 NSB endpoints in 6 console windows, plus Gateway, plus BackOffice, plus the website. Nine processes. Looks like "microservices".
- Compound B — `Website + AllInOne`: 6 NSB endpoints inside one `OmNomNom.AllInOne` console window, plus Gateway, BackOffice, website. Four processes. Looks like "a monolith".
- The codebase has not changed between the two runs. The user clicks through the same checkout in both. Same messages flow, the same saga coordinates, the same database files end up with the same rows.
- The point: **a service boundary is a unit of decision-making authority. Deployment topology is a separate axis.** Decisions about "how many processes" are operational. Decisions about "who owns this field" are architectural. They should be moved independently.
- Worth flagging the footgun on stage: don't run both compounds at the same time, because both processes would bind the same `.learningtransport/<EndpointName>/` queue folder and race. The header comment in [AllInOne's Program.cs](../src/OmNomNom.AllInOne/Program.cs) explains this — show it on slide.

**Demo script.**
1. Run Compound A. Show 9 console windows arranged. Click through a checkout. Pause on the saga's "ShipOrderRequest sent" log line — point at the Shipping window. Pause on the BackOffice email log line.
2. Stop everything. Switch compounds.
3. Run Compound B. Same click-through. Saga log line in the AllInOne window with `[Shipping]` prefix. Email log line in BackOffice.
4. Slide: *"What changed in the code? Nothing. Boundaries didn't move. The deployment did."*

---

## Optional later posts

These didn't make the main spine but are worth knowing about as follow-ups.

- **OpenTelemetry tracing across boundaries.** NSB 10.2's handler spans report the *original* convention-based handler type (not the generated adapter). In AllInOne you can wire one tracer provider and watch a single trace fan across all six boundaries in-process. In Compound A you'd need an OTel collector to stitch the same picture. The contrast is itself part of the boundary-vs-topology story.
- **The Rider compound mechanism.** A short post on the `.run.xml` format and how the topology toggle is just "pick the compound". Useful for readers who don't know JetBrains.
- **Why LearningTransport for a demo, not RabbitMQ or Azure Service Bus.** LearningTransport's queue-as-folder model means you can `tree .learningtransport/` and *see* the messages. Worth a paragraph at minimum, since attendees ask.
- **The pricing-boundary fix.** Walk through [docs/plan.md](plan.md) as a worked example of detecting and fixing a boundary violation in a real codebase. Good post for "what do I do when I find a leak after the fact?"

---

## State of the codebase (snapshot)

As of [PR #171](https://github.com/dvdstelt/OmNomNom/pull/171):

- **NServiceBus 10.2.0** across every endpoint.
- **`NServiceBus.Extensions.Hosting` is no longer referenced anywhere** in the solution. Replaced by `AddNServiceBusEndpoint` (PR #7633).
- **All 13 plain handlers are convention-based**: `[Handler]` attribute, no `IHandleMessages<T>` interface. Adapters are synthesised by the analyzer at compile time (PR #7641).
- **Both sagas (`ShippingPolicy`, `ReturnPolicy`)** have `[Saga]` and explicit `AddSaga<T>()` registration. Saga start/continue declarations stay on the `IAmStartedByMessages<T>` / `IHandleMessages<T>` interfaces because the runtime needs that semantic.
- **`OmNomNom.AllInOne`** exists as a console host running all six endpoints in one process. The new Rider compound `Website + AllInOne` lives alongside `Website + Endpoints`.
- **Database seeding** runs as an `IHostedLifecycleService.StartingAsync` in each endpoint's seeder, registered inside the `services.AddXEndpoint()` extension method. No explicit initializer calls in any `Program.cs`.
- **V2 deterministic host-ID** (XxHash128) opted in via `src/Directory.Build.props` for every project.

Open items (none blocking):

- OpenTelemetry not wired up. Optional next PR.
- `BackOffice` and `CompositionGateway` still scan assemblies for handlers — harmless because registration is explicit, but inconsistent with the six message endpoints. One-line cleanup each.
- The `ApplyDiscoveredRoutingConfigurators` reflection walk in `EndpointConfigurationExtensions.Configure(...)` could be replaced with explicit routing registration now that everything else is explicit. Minor.
