---
title: OmNomNom project anchor
tags:
  - omnomnom
  - anchor
  - overview
  - workflowcomposer
lifecycle: permanent
createdAt: '2026-05-09T19:53:38.789Z'
updatedAt: '2026-05-09T19:53:38.789Z'
role: reference
alwaysLoad: true
project: https-github-com-dvdstelt-omnomnom
projectName: OmNomNom
memoryVersion: 1
---
OmNomNom is a conference/teaching demo for UI Composition and service-boundary patterns built on NServiceBus 10. Each domain boundary owns its own data and communicates with peers exclusively via messages.

## Service boundaries

- **Catalog** — products, in-progress carts, accepted orders, inventory
- **Finance** — pricing, billing addresses, totals
- **Shipping** — shipping addresses, delivery options, fulfillment saga
- **PaymentInfo** — payment tokens
- **Marketing** — ratings, trending data
- **Branding** — site chrome / SvelteKit frontend (no backend service)
- **IT/OPS** — `CompositionGateway` and the `WorkflowComposer` library; cross-cutting capabilities the domain boundaries plug into

## Stack

- NServiceBus 10.1.4, LearningTransport, custom SQLite persister
  (`NServiceBusContrib.Persistence.Sqlite{,.TransactionalSession}` 0.1.0-beta.1)
- EF Core 10 over SQLite, one DB per backend endpoint
- ServiceComposer for read-side composition; WorkflowComposer (in-tree, this project) for write-side composition
- SvelteKit frontend at `src/website`

## Where to look first

- `AGENTS.md` at the repo root — top-level project guidance
- `docs/workflowcomposer/` — README, concepts, getting-started, and the migration plan for the in-flight cache → workflow-store migration. The migration plan is the canonical "what we did, why, and what's left" record
- `docs/plan.md` — separate, unrelated plan for the historical-pricing fix

## Current state of the WorkflowComposer migration

- Phases 1-3 complete: WorkflowComposer library + SQLite backend exists; Catalog, Finance, and Shipping have migrated their checkout slices off `IDistributedCache` to `IWorkflowStore`
- Phases 4-6 pending: PaymentInfo migration, then `WorkflowSubmitter` + `Checkout.Endpoint` for atomic submit, then cleanup
- Submit is intentionally not yet wired: every migrated handler still dispatches its NServiceBus command directly via `IMessageSession` as a transitional dual-write. `docs/workflowcomposer/migration-plan.md` lists every loose end and which phase removes it
