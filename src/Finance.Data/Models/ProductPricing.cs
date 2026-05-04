namespace Finance.Data.Models;

// Anything with a list price and a percentage discount can flow through
// the centralized pricing rule below. Implemented by Product (the
// catalog's master record) and OrderItem (the per-line snapshot
// captured when a cart is submitted).
public interface IPriced
{
    decimal Price { get; }
    decimal Discount { get; }
}

// Centralized pricing rule for the Finance boundary. Mirrors the
// frontend in src/website/src/Finance/effectivePrice.js; if you change
// one rule, change the other.
//
// Discount is a percentage off the list price (e.g. Discount = 25 means
// "25% off"). Values outside (0, 100) are treated as no discount so a
// data error can't produce a negative or inverted total.
public static class ProductPricing
{
    const decimal MaxDiscountPercent = 100m;

    public static bool HasDiscount(this IPriced item) =>
        item.Discount > 0 && item.Discount < MaxDiscountPercent;

    public static decimal DiscountAmount(this IPriced item) =>
        item.HasDiscount() ? item.Price * item.Discount / 100 : 0;

    public static decimal EffectivePrice(this IPriced item) =>
        item.Price - item.DiscountAmount();
}
