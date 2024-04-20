using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Finance.ServiceComposition.Checkout;

public class AddressSubmitHandler : ICompositionRequestsHandler
{
    [HttpPost("/cart/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        throw new NotImplementedException();
    }
}