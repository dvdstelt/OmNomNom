# Logical Service Boundaries

Each service boundary owns a specific slice of the business domain and its data. Services never share a database - each maintains its own store with only the fields it needs, even when multiple services hold data about the same conceptual entity (e.g. a product or a delivery option).

## Catalog

Responsible for the product catalogue and inventory.

**Products:** name, description, image, category
**Inventory:** stock levels tracked as a series of deltas (positive = added, negative = reserved)
**Orders:** which items were ordered and in what quantity

## Finance

Responsible for pricing and billing.

**Products:** price and discount per product
**Orders:** billing address, selected delivery option, ordered items with their prices
**Delivery options:** price per delivery option (Standard $2, Expedited $6, Priority $14)

## Shipping

Responsible for logistics and fulfilment.

**Orders:** shipping address, selected delivery option
**Delivery options:** name and description per option (e.g. "Standard shipping - 7-10 business days")

## PaymentInfo

Responsible for payment instruments.

**Credit cards:** card holder, card type, last digits, expiry, currency, payment provider token
**Orders:** which credit card was used for an order

## Marketing

Responsible for product reputation data.

**Products:** star rating and review count

---

## How the split works in practice

The delivery option is a clear example: when a customer selects a shipping method, Shipping owns what the option *is* (name, description, lead time) and Finance owns what it *costs* (price). Neither service reaches into the other's data. The complete picture is assembled at read time by the CompositionGateway, which asks both services to contribute their piece to a single HTTP response.

The same applies to products: Catalog owns the master data a customer sees (name, image, description), Finance owns the price, and Marketing owns the rating. Each service contributes only its own fields to the product page.
