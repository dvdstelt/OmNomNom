using Finance.Data.Models;

namespace Finance.Data.Domain;

// Single source of truth for what a delivery option actually costs on
// a given order. Callers compute the items subtotal in whatever way
// fits their context (live composer slice, persisted Order, etc.) and
// ask for the effective price; this class never touches the DB or the
// workflow store.
public static class ShippingFees
{
    public static decimal EffectivePrice(DeliveryOption option, decimal itemsSubtotal)
        => option.FreeShippingThreshold is { } threshold && itemsSubtotal > threshold
            ? 0m
            : option.Price;
}
