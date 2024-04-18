using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Temporary;
using Temporary.Caching;

namespace OmNomNom.Website.ViewModelComposition;

public class ShippingSubmit : ICompositionRequestsHandler
{
    private readonly CacheHelper storage;

    public ShippingSubmit(CacheHelper storage)
    {
        this.storage = storage;
    }

    [HttpPost("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var res = await request.Bind<ShippingModel>();
        var order = await storage.GetCart(res.orderId);
        order.DeliveryOptionId = res.Detail.DeliveryOptionId;
    }
}