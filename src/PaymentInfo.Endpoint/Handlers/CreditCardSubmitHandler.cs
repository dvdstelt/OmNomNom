using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data;
using PaymentInfo.Data.Models;
using PaymentInfo.Endpoint.Messages.Commands;
using PaymentInfo.Endpoint.Messages.Events;

namespace PaymentInfo.Endpoint.Handlers;

public class SubmitPaymentInfoHandler(PaymentInfoDbContext dbContext) : IHandleMessages<SubmitPaymentInfo>
{
    public async Task Handle(SubmitPaymentInfo message, IMessageHandlerContext context)
    {
        var ct = context.CancellationToken;

        var creditCard = await dbContext.CreditCards
            .FirstOrDefaultAsync(s =>
                s.CreditCardId == message.CreditCardId && s.CustomerId == message.CustomerId, ct);

        if (creditCard == null)
        {
            await context.Publish(new FraudDetection
            {
                CreditCardId = message.CreditCardId,
                CustomerId = message.CustomerId,
                OrderId = message.OrderId,
                FraudMessage = "Customer and credit card could not be matched!"
            });
            return;
        }

        var order = await dbContext.Orders
            .FirstOrDefaultAsync(s => s.OrderId == message.OrderId, ct);
        if (order == null)
        {
            order = new Order { OrderId = message.OrderId };
            dbContext.Orders.Add(order);
        }
        order.CreditCardId = message.CreditCardId;

        await dbContext.SaveChangesAsync(ct);
    }
}
