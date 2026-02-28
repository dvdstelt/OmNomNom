# Plan: Fix Price Boundary Violation and Implement Historical Prices

## Overview

Two related problems exist in the order submission flow:

1. **Price leaks across the client boundary.** The `ShoppingCartItem` DTO in `Finance.ServiceComposition/Checkout/OrderSubmitHandler.cs` accepts a `Price` field from the HTTP request body. Finance owns pricing and must never trust a client-supplied price. The field is currently silently ignored, but its presence is a design smell and a latent risk.

2. **Stale price at order time.** `Finance.Endpoint/Handlers/SubmitOrderItemsHandler.cs` reads the *current* price from the product catalogue at the moment the `SubmitOrderItems` message is processed - not the price the customer saw when they added the item to their cart. The `SubmitOrderItems` command already has a `PriceId` field on `OrderItem` for this purpose, but it is never populated or used.

---

## Phase 1 - Remove client-supplied price from the DTO

**Goal:** eliminate `Price` from the shopping cart submission request so no price data can enter the Finance service from the outside.

### Steps

- [ ] Remove the `Price` property from the private `ShoppingCartItem` class in `Finance.ServiceComposition/Checkout/OrderSubmitHandler.cs`.
- [ ] Confirm no other code in `OrderSubmitHandler` references the removed field.
- [ ] Verify the website does not rely on the Finance service echoing `Price` back in the cart submission response (composition response model is dynamic, so check the frontend cart service).

---

## Phase 2 - Implement historical prices via PriceId

**Goal:** record a price snapshot when Finance publishes product data so that the exact price the customer saw is stored and used when the order is finalised.

### Key Constraints

- `PriceId` is a Finance-internal identifier. The client passes it as an opaque token; it never supplies a price amount.
- Finance must validate that a received `PriceId` belongs to the correct `ProductId`. A mismatched or unknown `PriceId` is rejected.
- Historical price records are immutable once created.

### Steps

#### 2a - Add a PriceHistory entity to Finance.Data

- [ ] Create `Finance.Data/Models/PriceHistory.cs` with fields: `PriceId` (Guid), `ProductId` (Guid), `Price` (decimal), `Discount` (decimal), `ValidFrom` (DateTime UTC).
- [ ] Register the collection in `FinanceDbContext`.
- [ ] Add seed data that creates initial `PriceHistory` records matching the existing product prices.

#### 2b - Expose PriceId on product responses

- [ ] In `Finance.ServiceComposition/Products/ProductLoadedSubscriber.cs`, look up (or create on demand) a `PriceHistory` record for the product and set `vm.PriceId` alongside the existing `vm.Price` and `vm.Discount`.
- [ ] Do the same in `Finance.ServiceComposition/Products/ProductsLoadedSubscriber.cs`.
- [ ] In `Finance.ServiceComposition/Orders/CartLoadedSubscriber.cs`, set `vm.PriceId` per cart item so the frontend can echo it back.

#### 2c - Pass PriceId through the cart submission

- [ ] In the website, store `PriceId` per cart item (received from the product/cart endpoints) and include it in the `POST /cart/{orderId}` request body alongside `ProductId` and `Quantity`.
- [ ] In `Finance.ServiceComposition/Checkout/OrderSubmitHandler.cs`, read `PriceId` from the deserialized `ShoppingCartItem` and populate it on the `OrderItem` in the `SubmitOrderItems` command.

#### 2d - Use PriceId in the Finance endpoint handler

- [ ] In `Finance.Endpoint/Handlers/SubmitOrderItemsHandler.cs`, replace the current-price lookup with a `PriceHistory` lookup by `PriceId`.
- [ ] Validate that the retrieved `PriceHistory` record's `ProductId` matches the submitted `item.ProductId`; reject (log and skip or publish a fault event) if it does not.
- [ ] Remove the `// NOTE: Never ever do this!` comment once the correct behaviour is in place.

#### 2e - Verify the summary and email flows still work

- [ ] `Finance.ServiceComposition/Checkout/SummaryLoadedSubscriber.cs` reads `order.Items[].Price` from the database; confirm this price now reflects the historical snapshot rather than the current price.
- [ ] `Finance.ServiceComposition/Email/OrderSummary.cs` calculates `TotalPrice` from order item prices; confirm the values are correct end-to-end.
- [ ] Run through the full checkout flow manually (or via integration tests) to confirm prices shown in the summary and email match the prices shown on the product page.
