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
            // Finance hasn't processed SubmitOrderItems yet - fall
            // back to the in-flight slice. The slice carries no
            // prices (those are domain-owned and only get populated
            // by Finance's handler), so the totals come out as 0
            // until the message is dispatched and processed.
            var slice = await workflow.Read<OrderItemsSlice>(orderId, OrderItemsWorkflowSlice.Key, ct);
            order = new Order { OrderId = orderId };
            if (slice is not null)
            {
                foreach (var line in slice.Items)
                {
                    order.Items.Add(new OrderItem
                    {
                        OrderId = orderId,
                        ProductId = line.ProductId,
                        Quantity = line.Quantity
                    });
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
