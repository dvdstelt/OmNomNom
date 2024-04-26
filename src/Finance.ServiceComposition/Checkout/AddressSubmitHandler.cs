using Finance.Data.Models;
using Finance.Endpoint.Messages.Commands;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Checkout;

public class AddressSubmitHandler(IMessageSession messageSession) : ICompositionRequestsHandler
{
    [HttpPost("/buy/address/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var submitted = await request.Bind<OrderAddressDetails>();

        var message = new SubmitBillingAddress()
        {
            OrderId = submitted.OrderId,
            FullName = submitted.Details.BillingAddress.FullName,
            Street = submitted.Details.BillingAddress.Street,
            ZipCode = submitted.Details.BillingAddress.ZipCode,
            Town = submitted.Details.BillingAddress.Town,
            Country = submitted.Details.BillingAddress.Country,
        };

        await messageSession.Send(message);
    }

    class OrderAddressDetails
    {
        [FromRoute] public Guid OrderId { get; set; }
        [FromBody] public OrderAddress Details { get; set; }

        public class OrderAddress
        {
            public Address BillingAddress { get; set; }
        }
    }
}