using Finance.Data;
using Finance.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Checkout;

public class AddressHandler(FinanceDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);

        var orderCollection = dbContext.Database.GetCollection<Order>();
        var order = orderCollection.Query().Where(s => s.OrderId == orderId).SingleOrDefault();

        // If there is no order yet, we retrieve the address from the previous order.
        Address address;
        if (order == null)
            address = RetrieveAddressFromPreviousOrder(orderId);
        else
            address = order.BillingAddress;

        vm.BillingAddress = address;

        return Task.CompletedTask;
    }

    Address RetrieveAddressFromPreviousOrder(Guid orderId)
    {
        // Hahaha, we don't look it up, we just return the address
        // of the best football team in The Netherlands!
        return new Address()
        {
            FullName = "Dennis van der Stelt",
            Street = "Van Zandvlietplein 1",
            ZipCode = "3077 AA",
            Town = "Rotterdam",
            Country = "The Netherlands"
        };
    }
}