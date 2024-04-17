using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Temporary;

namespace OmNomNom.Website.ViewModelComposition;

public class ProductHandler : ICompositionRequestsHandler
{
    [HttpGet("/product/{productId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var productId = (string)request.HttpContext.GetRouteData().Values["productId"]!;

        vm.UrlId = productId;
        vm.Product = Products.GetProductById(Guid.Parse(productId));
        
        return Task.CompletedTask;
    }
}