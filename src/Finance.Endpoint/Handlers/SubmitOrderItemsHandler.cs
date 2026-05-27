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

        // NOTE: Never ever do this! Don't retrieve _current_ price after submitting the order.
        // Instead, use attached PriceId. But that isn't implemented yet.
        var productIds = message.Items.Select(i => i.ProductId).ToList();
        var products = await dbContext.Products
            .Where(p => productIds.Contains(p.ProductId))
            .ToDictionaryAsync(p => p.ProductId, context.CancellationToken);

        foreach (var item in message.Items)
        {
            var product = products[item.ProductId];
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                BillableQuantity = item.OrderedQuantity,
                Price = product.Price,
                Discount = product.Discount
            });
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
