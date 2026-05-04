// Centralized pricing rule for the Finance boundary. Every consumer
// (Finance microviews, the cart row in Catalog, and the cart route in
// Branding) MUST go through here so the displayed unit price, the row
// subtotal, and the cart total can never disagree.
//
// A discount applies only when it is strictly between 0 and the list
// price. A discount of 0 means "no promotion"; a discount equal to or
// greater than the price would be a data error, and silently using it
// would inflate the cart total. In both cases we fall back to the list
// price.

export function hasDiscount(price, discount) {
  return discount > 0 && discount < price;
}

function pair(item) {
  return [Number(item?.price ?? 0), Number(item?.discount ?? 0)];
}

export function effectivePrice(item) {
  const [price, discount] = pair(item);
  return hasDiscount(price, discount) ? discount : price;
}

// Dollar savings off the list price, or 0 when no valid discount applies.
export function discountAmount(item) {
  const [price, discount] = pair(item);
  return hasDiscount(price, discount) ? price - discount : 0;
}

// Savings as a rounded integer percentage (e.g. 25 for "Save 25%"),
// or 0 when no valid discount applies.
export function discountPercent(item) {
  const [price, discount] = pair(item);
  return hasDiscount(price, discount)
    ? Math.round(((price - discount) / price) * 100)
    : 0;
}
