using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PaymentInfo.Data;
using PaymentInfo.Data.Models;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

public class SummaryHandler(PaymentInfoDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var orderCollection = dbContext.Database.GetCollection<Order>();
        var creditCardCollection = dbContext.Database.GetCollection<Data.Models.CreditCard>();
        var order = orderCollection.Query().Where(s => s.OrderId == orderId).SingleOrDefault();
        var creditCard = creditCardCollection.Query().Where(s => s.CreditCardId == order.CreditCardId)
            .SingleOrDefault();

        var vm = request.GetComposedResponseModel();
        vm.CreditCardLastDigits = creditCard.LastDigits;
        vm.CreditCardType = creditCard.CardType;
    }
}