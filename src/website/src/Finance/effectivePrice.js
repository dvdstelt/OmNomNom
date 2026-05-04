// Centralized pricing rule for the Finance boundary. Every consumer
// (Finance microviews, the cart row in Catalog, and the cart route in
// Branding) MUST go through here so the displayed unit price, the row
// subtotal, and the cart total can never disagree.
//
// `discount` is a percentage off the list price (e.g. discount=25 means
// "25% off"). Values outside (0, 100) are treated as no discount so a
// data error can't produce a negative or inverted total. This mirrors
// Finance.Data.Models.ProductPricing on the backend; if you change one
// rule, change the other.

const MAX_DISCOUNT_PERCENT = 100;

export function hasDiscount(discount) {
  return discount > 0 && discount < MAX_DISCOUNT_PERCENT;
}

function pair(item) {
  return [Number(item?.price ?? 0), Number(item?.discount ?? 0)];
}

// Dollar savings off the list price, or 0 when no valid discount applies.
export function discountAmount(item) {
  const [price, discount] = pair(item);
  return hasDiscount(discount) ? (price * discount) / 100 : 0;
}

export function effectivePrice(item) {
  const [price] = pair(item);
  return price - discountAmount(item);
}

// The discount expressed as a rounded integer percentage (e.g. 25 for
// "Save 25%"), or 0 when no valid discount applies.
export function discountPercent(item) {
  const [, discount] = pair(item);
  return hasDiscount(discount) ? Math.round(discount) : 0;
}
