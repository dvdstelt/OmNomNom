using Finance.Data;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using Microsoft.EntityFrameworkCore;
using OrderItem = Finance.Data.Models.OrderItem;

namespace Finance.Endpoint.Handlers;

public class SubmitOrderItemsHandler(FinanceDbContext dbContext) : IHandleMessages<SubmitOrderItems>
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

        foreach (var item in message.Items)
        {
            // NOTE: Never ever do this! Don't retrieve _current_ price after submitting the order.
            // Instead, use attached PriceId. But that isn't implemented yet.
            var product = await dbContext.Products
                .SingleAsync(s => s.ProductId == item.ProductId, context.CancellationToken);
            order.Items.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity,
                Price = product.Price,
                Discount = product.Discount
            });
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
