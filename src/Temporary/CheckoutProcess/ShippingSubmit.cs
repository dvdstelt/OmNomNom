using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Temporary;

namespace OmNomNom.Website.ViewModelComposition;

public class ShippingSubmit : ICompositionRequestsHandler
{
    private readonly OrderStorage storage;

    public ShippingSubmit(OrderStorage storage)
    {
        this.storage = storage;
    }

    [HttpPost("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var res = await request.Bind<ShippingModel>();
        var order = storage.GetOrder(res.orderId);
        order.DeliveryOptionId = res.Detail.DeliveryOptionId;
    }
}