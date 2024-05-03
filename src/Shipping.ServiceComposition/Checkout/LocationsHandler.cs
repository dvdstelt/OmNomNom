using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Shipping.Data;
using Shipping.Data.Models;

namespace Shipping.ServiceComposition.Checkout;

public class LocationsHandler(ShippingDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/locations")]
    public Task Handle(HttpRequest request)
    {
        var locationsCollection = dbContext.Database.GetCollection<Location>();

        var vm = request.GetComposedResponseModel();
        vm.Locations = locationsCollection.Query().ToArray();

        return Task.CompletedTask;
    }
}