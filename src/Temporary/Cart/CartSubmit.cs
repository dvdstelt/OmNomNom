using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Temporary;

namespace OmNomNom.Website.ViewModelComposition;

public class CartSubmit : ICompositionRequestsHandler
{
    readonly IHttpContextAccessor httpContextAccessor;
    private readonly OrderStorage storage;

    public CartSubmit(OrderStorage storage)
    {
        this.storage = storage;
    }

    [HttpPost("/cart/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var routeData = request.HttpContext.GetRouteData();
        var orderId = Guid.Parse(routeData.Values["orderId"] as string ?? throw new InvalidOperationException("OrderId can't be empty"));

        var res = await request.Bind<AddressModel>();
        var order = storage.GetOrder(orderId);
        order.FullName = res.Body.FullName;
        order.ShippingAddress = res.Body.ShippingAddress;
        order.BillingAddress = res.Body.BillingAddress;

        //use the content object instance as needed
    }
}
