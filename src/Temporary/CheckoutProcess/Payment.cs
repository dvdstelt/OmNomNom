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
        var orderId = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        return Task.CompletedTask;
    }
}