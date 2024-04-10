using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Payment : ICompositionRequestsHandler
{
    [HttpGet("/buy/payment/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially used address 
        var orderId = Guid.Parse("526f0a1d-2900-49ba-9d70-987e9f590b04");

        vm.OrderId = orderId;

        dynamic creditCard = new ExpandoObject();
        creditCard.Id = Guid.Parse("cd52a6bd-0aa5-495b-8021-b97e79910474");
        creditCard.Name = "MasterCard";
        creditCard.EndingIn = "2464";
        creditCard.Owner = "Dennis van der Stelt";
        creditCard.ExpiryDate = "09/2026";
        vm.CreditCard = creditCard;
        
        return Task.CompletedTask;
    }
}