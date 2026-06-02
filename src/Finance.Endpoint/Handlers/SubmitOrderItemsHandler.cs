using Finance.Data;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using Microsoft.EntityFrameworkCore;
using OrderItem = Finance.Data.Models.OrderItem;

namespace Finance.Endpoint.Handlers;

[Handler]
public class SubmitOrderItemsHandler(FinanceDbContext dbContext)
{
    public async Task Handle(SubmitOrderItems message, IMessageHandlerContext context)
    {
        var order = await dbContext.Orders
            .Include(o => o.Items)
            .FirstOrDefaultAsync(o => o.OrderId == message.OrderId, context.CancellationToken);

        if (order == null)
        {
            order = new Order { OrderId = message.OrderId };
            dbContext.Orders.Add(order);
        }
        else
        {
            order.Items.Clear();
        }

        // Bill against the exact price the customer saw: resolve the
        // immutable ProductPrice by the PriceId locked in at add-to-cart,
        // never the current price. A newer price added since then is a
        // separate row this lookup ignores.
        var priceIds = message.Items.Select(i => i.PriceId).ToList();
        var prices = await dbContext.ProductPrices
            .Where(p => priceIds.Contains(p.PriceId))
            .ToDictionaryAsync(p => p.PriceId, context.CancellationToken);

        foreach (var item in message.Items)
        {
            var price = prices[item.PriceId];
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                BillableQuantity = item.OrderedQuantity,
                PriceId = item.PriceId,
                Price = price.Price,
                Discount = price.Discount
            });
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
