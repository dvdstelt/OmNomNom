using Finance.Data;
using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using Microsoft.EntityFrameworkCore;

namespace Finance.Endpoint.Handlers;

public class SubmitBillingAddressHandler(FinanceDbContext dbContext) : IHandleMessages<SubmitBillingAddress>
{
    public async Task Handle(SubmitBillingAddress message, IMessageHandlerContext context)
    {
        var order = await dbContext.Orders
            .FirstOrDefaultAsync(s => s.OrderId == message.OrderId, context.CancellationToken);

        if (order == null)
        {
            order = new Order { OrderId = message.OrderId };
            dbContext.Orders.Add(order);
        }

        order.BillingAddress = new Address
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
