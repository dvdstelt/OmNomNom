using Catalog.ServiceComposition.Events;
using Finance.Data;
using Finance.Data.Models;
using Finance.ServiceComposition.Helpers;
using Finance.ServiceComposition.Workflow;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using WorkflowComposer;

namespace Finance.ServiceComposition.Checkout;

public class OnSummaryLoaded(FinanceDbContext dbContext, IWorkflowStore workflow) : ICompositionEventsHandler<SummaryLoaded>
{
    public async Task Handle(SummaryLoaded @event, HttpRequest request)
    {
        var orderId = @event.OrderId;
        var ct = request.HttpContext.RequestAborted;

        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(s => s.OrderId == orderId, ct);

        if (order == null)
        {
            // Finance hasn't processed SubmitOrderItems yet - on this
            // branch that's the normal pre-submit state, since
            // SubmitOrderItems is now dispatched together with
            // CompleteOrder when the customer clicks "Place order".
            // Fall back to the in-flight slice and stitch in the
            // current per-product Price/Discount from Finance.Products
            // so the summary shows real per-line and grand totals
            // instead of zeros. The eventually-persisted OrderItem will
            // be the snapshot at submit time; this is the preview.
            var slice = await workflow.Read<OrderItemsSlice>(orderId, OrderItemsWorkflowSlice.Key, ct);
            order = new Order { OrderId = orderId };
            if (slice is not null && slice.Items.Count > 0)
            {
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
            }
        }

        var totalPrice = 0m;

        foreach (var product in @event.Products)
        {
            var matchingProduct = order.Items.SingleOrDefault(s => s.ProductId == product.Key);
            if (matchingProduct is null) continue;
            product.Value.Price = matchingProduct.Price;
            product.Value.Discount = matchingProduct.Discount;
            totalPrice += matchingProduct.EffectivePrice() * matchingProduct.Quantity;
        }

        var vm = request.GetComposedResponseModel();
        DynamicHelper.TrySetTotalPrice(vm, totalPrice);
    }
}
