using PaymentInfo.Data;
using PaymentInfo.Data.Models;
using PaymentInfo.Endpoint.Messages.Commands;
using PaymentInfo.Endpoint.Messages.Events;

namespace PaymentInfo.Endpoint.Handlers;

public class SubmitPaymentInfoHandler(PaymentInfoDbContext dbContext) : IHandleMessages<SubmitPaymentInfo>
{
    readonly PaymentInfoDbContext dbContext = dbContext;

    public async Task Handle(SubmitPaymentInfo message, IMessageHandlerContext context)
    {
        var creditCardCollection = dbContext.Database.GetCollection<CreditCard>();
        var orderCollection = dbContext.Database.GetCollection<Order>();
        var creditCard = creditCardCollection.Query()
            .Where(s => s.CreditCardId == message.CreditCardId && s.CustomerId == message.CustomerId).Single();

        if (creditCard == null)
        {
            var alert = new FraudDetection()
            {
                CreditCardId = message.CreditCardId,
                CustomerId = message.CustomerId,
                OrderId = message.OrderId,
                FraudMessage = "Customer and credit card could not be matched!"
            };
            await context.Publish(alert);
        }

        var order = new Order()
        {
            OrderId = message.OrderId,
            CreditCardId = message.CreditCardId
        };
        orderCollection.Upsert(order);
    }
}