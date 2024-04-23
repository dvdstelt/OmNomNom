using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data.Models;
using Shipping.Endpoint.Messages.Commands;

namespace Shipping.ServiceComposition.Checkout;

public class AddressSubmitHandler(IMessageSession messageSession) : ICompositionRequestsHandler
{
    [HttpPost("/buy/address/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<OrderAddressDetails>();

        var message = new SubmitShippingAddress()
        {
            OrderId = submitted.OrderId,
            FullName = submitted.Details.ShippingAddress.FullName,
            Street = submitted.Details.ShippingAddress.Street,
            ZipCode = submitted.Details.ShippingAddress.ZipCode,
            Town = submitted.Details.ShippingAddress.Town,
            Country = submitted.Details.ShippingAddress.Country,
        };
    }

    class OrderAddressDetails
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public OrderAddress Details { get; set; }

        public class OrderAddress
        {
            public Address ShippingAddress { get; set; }
        }
    }
}