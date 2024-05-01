using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PaymentInfo.Data.Models;
using PaymentInfo.Endpoint.Messages.Commands;
using PaymentInfo.ServiceComposition.Helpers;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

public class PaymentSubmitHandler(IMessageSession messageSession, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpPost("/buy/payment/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var customerId = Guid.Parse("01093176-1308-493a-8f67-da5d278e2375");
        var submitted = await request.Bind<CreditCard>();

        var order = new Order()
        {
            CreditCardId = submitted.Body.CreditCardId,
            OrderId = submitted.OrderId
        };
        await cacheHelper.StoreOrder(order);

        var message = new SubmitPaymentInfo()
        {
            OrderId = submitted.OrderId,
            CreditCardId = submitted.Body.CreditCardId,
            CustomerId = customerId
        };

        await messageSession.Send(message);
    }

    class CreditCard
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public PaymentBody Body { get; set; }
    }

    class PaymentBody
    {
        public Guid CreditCardId { get; set; }
    }
}