using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Shipping : ICompositionRequestsHandler
{
    [HttpGet("/buy/shipping/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var orderId = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        return Task.CompletedTask;
    }
}