using Finance.Data;
using Finance.Data.Models;
using Finance.ServiceComposition.Workflow;
using Microsoft.EntityFrameworkCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Helpers;

// Materializes the line items for an order as Finance currently sees
// them, regardless of whether the order has been submitted yet.
//
// Composers run in two phases of the checkout. Pre-submit there is no
// persisted Order yet (SubmitOrderItems is dispatched together with
// CompleteOrder), so items live in the workflow slice and are priced
// against the PriceId locked into that slice at add-to-cart. Post-submit
// the persisted Order.Items is the snapshot at submit time. Both paths
// return OrderItems whose Price/Discount/Quantity feed the same
// downstream subtotal math.
public class OrderSubtotalReader(FinanceDbContext dbContext, IWorkflowStore workflow)
{
    public async Task<Order> GetOrderWithItemsAsync(Guid orderId, CancellationToken ct)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == orderId, ct);

        if (order is not null)
            return order;

        order = new Order { OrderId = orderId };

        var slice = await workflow.Read<OrderItemsSlice>(orderId, OrderItemsWorkflowSlice.Key, ct);
        if (slice is null || slice.Items.Count == 0)
            return order;

        var priceIds = slice.Items.Select(i => i.PriceId).ToList();
        var prices = await dbContext.ProductPrices
            .Where(p => priceIds.Contains(p.PriceId))
            .ToDictionaryAsync(p => p.PriceId, ct);

        foreach (var line in slice.Items)
        {
            var item = new OrderItem
            {
                OrderId = orderId,
                ProductId = line.ProductId,
                PriceId = line.PriceId,
                BillableQuantity = line.Quantity
            };
            if (prices.TryGetValue(line.PriceId, out var price))
            {
                item.Price = price.Price;
                item.Discount = price.Discount;
            }
            order.Items.Add(item);
        }

        return order;
    }

    public async Task<decimal> GetItemsSubtotalAsync(Guid orderId, CancellationToken ct)
    {
        var order = await GetOrderWithItemsAsync(orderId, ct);
        return order.Items.Sum(i => i.EffectivePrice() * i.BillableQuantity);
    }
}
