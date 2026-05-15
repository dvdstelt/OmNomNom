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
// against the current Finance.Products row. Post-submit the persisted
// Order.Items is the snapshot at submit time. Both paths return
// OrderItems whose Price/Discount/Quantity feed the same downstream
// subtotal math.
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

        var productIds = slice.Items.Select(i => i.ProductId).ToList();
        var prices = await dbContext.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToDictionaryAsync(p => p.ProductId, ct);

        foreach (var line in slice.Items)
        {
            var item = new OrderItem
            {
                OrderId = orderId,
                ProductId = line.ProductId,
                Quantity = line.Quantity
            };
            if (prices.TryGetValue(line.ProductId, out var product))
            {
                item.Price = product.Price;
                item.Discount = product.Discount;
            }
            order.Items.Add(item);
        }

        return order;
    }

    public async Task<decimal> GetItemsSubtotalAsync(Guid orderId, CancellationToken ct)
    {
        var order = await GetOrderWithItemsAsync(orderId, ct);
        return order.Items.Sum(i => i.EffectivePrice() * i.Quantity);
    }
}
