namespace Finance.Data.Models;

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

    public static bool HasDiscount(this Product product) =>
        product.Discount > 0 && product.Discount < MaxDiscountPercent;

    public static decimal DiscountAmount(this Product product) =>
        product.HasDiscount() ? product.Price * product.Discount / 100 : 0;

    public static decimal EffectivePrice(this Product product) =>
        product.Price - product.DiscountAmount();
}
