using Microsoft.EntityFrameworkCore;
using Shipping.Data;
using Shipping.Data.Models;
using Shipping.Endpoint.Messages.Commands;

namespace Shipping.Endpoint.Handlers;

public class SubmitShippingAddressHandler(ShippingDbContext dbContext) : IHandleMessages<SubmitShippingAddress>
{
    public async Task Handle(SubmitShippingAddress message, IMessageHandlerContext context)
    {
        var order = await dbContext.Orders
            .FirstOrDefaultAsync(s => s.OrderId == message.OrderId, context.CancellationToken);

        if (order == null)
        {
            order = new Order { OrderId = message.OrderId };
            dbContext.Orders.Add(order);
        }

        order.Address = new Address
        {
            FullName = message.FullName,
            Street = message.Street,
            ZipCode = message.ZipCode,
            Town = message.Town,
            Country = message.Country
        };

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
