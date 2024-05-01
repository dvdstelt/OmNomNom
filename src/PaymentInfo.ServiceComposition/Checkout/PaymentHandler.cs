using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using PaymentInfo.Data;
using PaymentInfo.Data.Models;
using ServiceComposer.AspNetCore;

namespace PaymentInfo.ServiceComposition.Checkout;

public class PaymentHandler(PaymentInfoDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/payment/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var orderCollection = dbContext.Database.GetCollection<Order>();
        var order = orderCollection.Query().Where(s => s.OrderId == orderId).SingleOrDefault();

        vm.CreditCardId = order?.CreditCardId;

        return Task.CompletedTask;
    }
}