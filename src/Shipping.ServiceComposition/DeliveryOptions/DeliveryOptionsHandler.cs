using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Data.Models;
using Shipping.ServiceComposition.Events;

namespace Shipping.ServiceComposition.DeliveryOptions;

public class DeliveryOptionsHandler : ICompositionRequestsHandler
{
    private readonly ShippingDbContext dbContext;

    public DeliveryOptionsHandler(ShippingDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    
    [HttpGet("/deliveryoptions")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();

        var collection = dbContext.Database.GetCollection<DeliveryOption>();
        var deliveryOptions = collection.Query().ToList();

        var optionsModel = Mapper.MapToDictionary(deliveryOptions);
        
        var context = request.GetCompositionContext();
        await context.RaiseEvent(new DeliveryOptionsLoaded()
        {
            DeliveryOptions = optionsModel
        });

        vm.Products = optionsModel;
    }
}