# Logical Service Boundaries

Each service boundary owns a specific slice of the business domain and its data. Services never share a database - each maintains its own store with only the fields it needs, even when multiple services hold data about the same conceptual entity (e.g. a product or a delivery option).

## Catalog

Responsible for the product catalogue and inventory.

| Entity | Fields owned |
|---|---|
| Product | `ProductId`, `Name`, `Description`, `ImageUrl`, `Category` |
| InventoryDelta | `ProductId`, `Delta`, `Timestamp` |
| Order | `OrderId` |
| OrderItem | `ProductId`, `Quantity` |

## Finance

Responsible for pricing and billing.

| Entity | Fields owned |
|---|---|
| Product | `ProductId`, `Price`, `Discount` |
| PriceHistory | `PriceId`, `ProductId`, `Price`, `Discount`, `ValidFrom` |
| Order | `OrderId`, `BillingAddress`, `DeliveryOptionId` |
| OrderItem | `ProductId`, `Quantity`, `Price` (snapshot at order time) |
| DeliveryOption | `DeliveryOptionId`, `Price` |

## Shipping

Responsible for logistics and fulfilment.

| Entity | Fields owned |
|---|---|
| Order | `OrderId`, `CustomerId`, `Address`, `DeliveryOptionId` |
| DeliveryOption | `DeliveryOptionId`, `Name`, `Description` |

## PaymentInfo

Responsible for payment instruments.

| Entity | Fields owned |
|---|---|
| CreditCard | `CreditCardId`, `CardHolder`, `CardType`, `LastDigits`, `Expiry`, `Currency`, `ProviderToken` |
| Order | `OrderId`, `CreditCardId` |

## Marketing

Responsible for product reputation data.

| Entity | Fields owned |
|---|---|
| Product | `ProductId`, `StarRating`, `ReviewCount` |

---

## How the split works in practice

The delivery option is a clear example: when a customer selects a shipping method, Shipping owns what the option *is* (name, description, lead time) and Finance owns what it *costs* (price). Neither service reaches into the other's data. The complete picture is assembled at read time by the CompositionGateway, which asks both services to contribute their piece to a single HTTP response.

The same applies to products: Catalog owns the master data a customer sees (name, image, description), Finance owns the price, and Marketing owns the rating. Each service contributes only its own fields to the product page.
