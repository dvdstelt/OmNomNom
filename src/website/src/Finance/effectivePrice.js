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

export function effectivePrice(item) {
  const price = Number(item?.price ?? 0);
  const discount = Number(item?.discount ?? 0);
  return hasDiscount(price, discount) ? discount : price;
}
