---
title: 'OmNomNom: prefer oversell + reconciliation over locking on append-only deltas'
tags:
  - catalog
  - inventory
  - concurrency
  - design-decision
lifecycle: permanent
createdAt: '2026-05-14T20:58:10.752Z'
updatedAt: '2026-05-14T20:58:10.752Z'
role: decision
alwaysLoad: false
project: https-github-com-dvdstelt-omnomnom
projectName: OmNomNom
memoryVersion: 1
---
When working with the inventory delta log in OmNomNom (Catalog's `InventoryDelta` table), do not propose pessimistic locking, serializable transactions, or `BEGIN IMMEDIATE` to prevent overselling on concurrent `CompleteOrder` handlers.

**Why:** the append-only delta design exists *specifically* to allow concurrent writers without contention. A serializable transaction would funnel every reservation through the SQLite writer lock and create congestion on hot products - the exact pathology the deltas are meant to avoid. The user prefers to oversell and reconcile out-of-band (e.g. when the warehouse reports an order it cannot fulfill) rather than pay the contention cost up front.

**How to apply:** in any Catalog refactor that touches stock checking and reservation, leave the read-sum / append-delta pair lock-free. Note overselling as a known, accepted trade-off and route reconciliation through downstream events / warehouse feedback. Lock-free optimistic schemes (e.g. append-then-compensate) are fine to suggest if they avoid central contention; row/table-level locks are not.
