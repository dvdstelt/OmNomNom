using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Data.Models;

namespace Shipping.ServiceComposition.Checkout;

public class AddressHandler(ShippingDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var orderId = Guid.Parse(orderIdString);
        var ct = request.HttpContext.RequestAborted;

        var order = await dbContext.Orders.FirstOrDefaultAsync(s => s.OrderId == orderId, ct);

        // If there is no order yet, we retrieve the address from the previous order.
        Address address;
        if (order?.Address == null)
            address = RetrieveAddressFromPreviousOrder(orderId);
        else
            address = order.Address;

        vm.ShippingAddress = address;
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
