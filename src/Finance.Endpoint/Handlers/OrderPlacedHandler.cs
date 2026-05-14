using Catalog.Endpoint.Messages.Events;
using Finance.Data;
using Microsoft.EntityFrameworkCore;

namespace Finance.Endpoint.Handlers;

// Marks any line items Catalog could not fulfil as Fulfilled=false so
// totals computed from the Order (in the email composer and anywhere
// else) only charge the customer for what actually shipped.
public class OrderPlacedHandler(FinanceDbContext dbContext) : IHandleMessages<OrderPlaced>
{
    public async Task Handle(OrderPlaced message, IMessageHandlerContext context)
    {
        if (message.UnfulfilledItems.Count == 0)
            return;

        var unfulfilledIds = message.UnfulfilledItems.Select(i => i.ProductId).ToHashSet();

        var items = await dbContext.OrderItems
            .Where(i => i.OrderId == message.OrderId && unfulfilledIds.Contains(i.ProductId))
            .ToListAsync(context.CancellationToken);

        foreach (var item in items)
        {
            item.Fulfilled = false;
        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
    }
}
