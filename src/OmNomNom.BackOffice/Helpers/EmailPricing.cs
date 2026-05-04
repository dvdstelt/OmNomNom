namespace OmNomNom.Website.Helpers;

// Local copy of the Finance pricing rule for use in the email Razor
// view. We intentionally don't reference Finance.Data here so domain
// logic stays inside its boundary; this helper just keeps the email
// numbers consistent with the rule defined in
// Finance.Data.Models.ProductPricing and the SPA's effectivePrice.js.
//
// Discount is a percentage off the list price. Values outside (0, 100)
// are treated as no discount so a data error can't produce a negative
// or inverted total.
public static class EmailPricing
{
    const decimal MaxDiscountPercent = 100m;

    public static bool HasDiscount(decimal discount) =>
        discount > 0 && discount < MaxDiscountPercent;

    public static decimal EffectivePrice(decimal price, decimal discount) =>
        HasDiscount(discount) ? price - price * discount / 100 : price;

    public static decimal DiscountAmount(decimal price, decimal discount) =>
        HasDiscount(discount) ? price * discount / 100 : 0;
}
