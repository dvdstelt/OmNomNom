using Catalog.Data;
using Catalog.Data.Models;
using Catalog.Endpoint.Messages.Commands;
using Microsoft.EntityFrameworkCore;
using OrderItem = Catalog.Data.Models.OrderItem;

namespace Catalog.Endpoint.Handlers;

public class SubmitOrderItemsHandler(CatalogDbContext dbContext) : IHandleMessages<SubmitOrderItems>
{
    public async Task Handle(SubmitOrderItems message, IMessageHandlerContext context)
    {
        // Idempotent: if an order with this OrderId already exists, replace
        // its line items rather than creating a duplicate. EF Core's
        // composite (OrderId, ProductId) PK on OrderItem would otherwise
        // throw on a duplicate insert.
        var order = await dbContext.Orders
            .Include(o => o.Products)
            .FirstOrDefaultAsync(o => o.OrderId == message.OrderId, context.CancellationToken);

        if (order == null)
        {
            order = new Order { OrderId = message.OrderId };
            dbContext.Orders.Add(order);
        }
        else
        {
            order.Products.Clear();
        }

        foreach (var item in message.Items)
        {
            order.Products.Add(new OrderItem
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            });
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
