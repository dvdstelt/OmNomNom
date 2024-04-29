using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Data.Models;
using Shipping.ServiceComposition.DeliveryOptions;
using Shipping.ServiceComposition.Events;

namespace Shipping.ServiceComposition.Checkout;

public class DeliveryOptionsHandler(ShippingDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/buy/shipping/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();

        // Use OrderId to retrieve address from cache and figure out if shipping is international

        var collection = dbContext.Database.GetCollection<DeliveryOption>();
        var deliveryOptions = collection.Query().ToList();

        var optionsModel = Mapper.MapToDictionary(deliveryOptions);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliveryOptionsLoaded()
        {
            DeliveryOptions = optionsModel
        });

        vm.DeliveryOptions = optionsModel.Select(kvp => kvp.Value);
    }
}