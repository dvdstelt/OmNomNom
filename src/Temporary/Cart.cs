using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Cart : ICompositionRequestsHandler
{
    [HttpGet("/cart/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var orderId = (string)request.HttpContext.GetRouteData().Values["orderId"]!;
        var vm = request.GetComposedResponseModel();

        vm.OrderId = orderId;
        vm.Product = new ExpandoObject();
        vm.Product.Id = "09390028";
        vm.Product.Name = "Pizza";
        vm.Product.Price = 5;

        return Task.CompletedTask;
    }
}