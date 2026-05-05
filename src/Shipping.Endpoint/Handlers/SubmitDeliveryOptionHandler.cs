using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Data.Models;
using Shipping.Endpoint.Messages.Commands;

namespace Shipping.Endpoint.Handlers;

public class SubmitDeliveryOptionHandler(ShippingDbContext dbContext) : IHandleMessages<SubmitDeliveryOption>
{
    public async Task Handle(SubmitDeliveryOption message, IMessageHandlerContext context)
    {
        var order = await dbContext.Orders
            .FirstOrDefaultAsync(s => s.OrderId == message.OrderId, context.CancellationToken);

        if (order == null)
        {
            order = new Order { OrderId = message.OrderId };
            dbContext.Orders.Add(order);
        }

        order.DeliveryOptionId = message.DeliveryOptionId;

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
