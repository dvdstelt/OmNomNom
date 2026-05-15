using Catalog.Endpoint.Messages.Events;
using Finance.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Endpoint.Handlers;

// When Catalog could not fulfil any line, every item on the Finance
// order is marked unfulfilled so the cancellation email shows what was
// missed and any total computation comes out at zero.
public class OrderCancelledHandler(FinanceDbContext dbContext) : IHandleMessages<OrderCancelled>
{
    public async Task Handle(OrderCancelled message, IMessageHandlerContext context)
    {
        var items = await dbContext.OrderItems
            .Where(i => i.OrderId == message.OrderId)
            .ToListAsync(context.CancellationToken);

        foreach (var item in items)
        {
            item.Fulfilled = false;
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
