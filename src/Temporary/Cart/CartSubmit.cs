using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using System.Text.Json;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class CartSubmit : ICompositionRequestsHandler
{
    readonly IHttpContextAccessor httpContextAccessor;

    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var routeData = request.HttpContext.GetRouteData();
        var orderId = Guid.Parse(routeData.Values["orderId"] as string ?? throw new InvalidOperationException("OrderId can't be empty"));

        var res = await request.Bind<RequestModel>();

        //use the content object instance as needed
    }
}

// id: "",
// street: "",
// zipCode: "",
// town: "",
// country: "",
