using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data;
using PaymentInfo.ServiceComposition.Helpers;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

public class SummaryComposer(PaymentInfoDbContext dbContext, CacheHelper cacheHelper) : ICompositionRequestsHandler
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var row = await (
            from o in dbContext.Orders
            where o.OrderId == orderId
            from c in dbContext.CreditCards.Where(c => c.CreditCardId == o.CreditCardId).DefaultIfEmpty()
            select new { Order = o, CreditCard = c })
            .FirstOrDefaultAsync(ct);

        Data.Models.CreditCard? creditCard;
        if (row != null)
        {
            creditCard = row.CreditCard;
        }
        else
        {
            // Order not yet persisted; fall back to the in-progress cache.
            var order = await cacheHelper.GetOrder(orderId);
            creditCard = await dbContext.CreditCards
                .FirstOrDefaultAsync(s => s.CreditCardId == order.CreditCardId, ct);
        }

        var vm = request.GetComposedResponseModel();
        vm.CreditCardLastDigits = creditCard?.LastDigits;
        vm.CreditCardType = creditCard?.CardType;
    }
}
