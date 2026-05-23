using Catalog.Endpoint.Messages.Events;
using Finance.Data;
using Finance.Data.Domain;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Events;
using Microsoft.EntityFrameworkCore;

namespace Finance.Endpoint.Handlers;

// Sole entry point for billing an accepted order. Receives the full
// fulfilment outcome from Catalog so the charge can be computed
// against the items that actually shipped, never against the original
// cart, then publishes PaymentSucceeded once the amount is committed
// to the Order. There is no separate OrderAcceptedHandler -
// publishing PaymentSucceeded on OrderAccepted would race this
// handler and bill the customer for items Catalog could not fulfil.
[Handler]
public class OrderPlacedHandler(FinanceDbContext dbContext)
{
    public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        var ct = context.CancellationToken;

        var order = await dbContext.Orders
            .Include(o => o.Items)
            .SingleAsync(o => o.OrderId == message.OrderId, ct);

        var unfulfilledIds = message.UnfulfilledItems.Select(i => i.ProductId).ToHashSet();
        foreach (var item in order.Items)
        {
            if (unfulfilledIds.Contains(item.ProductId))
                item.Fulfilled = false;
        }

        order.ChargedAmount = await ComputeChargedAmountAsync(order, ct);

        await dbContext.SaveChangesAsync(ct);

        await context.Publish(new PaymentSucceeded { OrderId = message.OrderId });
    }

    async Task<decimal> ComputeChargedAmountAsync(Order order, CancellationToken ct)
    {
        var itemsTotal = order.Items
            .Where(i => i.Fulfilled)
            .Sum(i => i.EffectivePrice() * i.Quantity);

        // Shipping is only charged when at least one line shipped. A
        // partial order still pays the full delivery fee - we packed
        // and dispatched a box, just with fewer items in it. The free
        // shipping threshold is re-evaluated against the fulfilled
        // subtotal, so a partial that drops the order below the
        // threshold correctly re-introduces the shipping charge.
        if (itemsTotal == 0 || order.DeliveryOptionId is null)
            return itemsTotal;

        var deliveryOption = await dbContext.DeliveryOptions
            .SingleAsync(d => d.DeliveryOptionId == order.DeliveryOptionId, ct);

        return itemsTotal + ShippingFees.EffectivePrice(deliveryOption, itemsTotal);
    }
}
