using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Address : ICompositionRequestsHandler
{
    [HttpGet("/buy/address/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        
        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially used address 
        var orderId = Guid.Parse("526f0a1d-2900-49ba-9d70-987e9f590b04");
        
        vm.OrderId = orderId;
        vm.FullName = "Dennis van der Stelt";
        vm.ShippingAddress =
            CreateAddress(Guid.NewGuid(), "Stenen Kamer 16", "2952ED", "Alblasserdam", "The Netherlands");
        vm.IsBillingAddressSame = true;
        // Since BillingAddress == ShippingAddress, the UI should show a checkbox.
        // When checkbox is turned off, it should show the same fields as ShippingAddress, except empty
        vm.BillingAddress = vm.ShippingAddress;
        
        return Task.CompletedTask;
    }
    
    dynamic CreateAddress(Guid id, string street, string zipcode, string town, string country)
    {
        dynamic address = new ExpandoObject();
        address.Id = id;
        address.Street = street;
        address.Zipcode = zipcode;
        address.Town = town;
        address.Country = country;
        return address;
    }
}