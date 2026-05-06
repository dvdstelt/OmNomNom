using Finance.Data;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using Microsoft.EntityFrameworkCore;

namespace Finance.Endpoint.Handlers;

public class SubmitDeliveryOptionHandler(FinanceDbContext dbContext) : IHandleMessages<SubmitDeliveryOption>
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
