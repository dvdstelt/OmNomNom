using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Product : ICompositionRequestsHandler
{
    [HttpGet("/product/{productId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var productId = (string)request.HttpContext.GetRouteData().Values["productId"]!;

        vm.UrlId = productId;
        vm.Product = CreateProduct(Guid.Parse("ff899e9d-4033-48d4-b189-e6ef4a3dc25b"), "Fremont Stout", "https://www.surprisefactory.nl/media/48245/zelf-bier-brouwen.jpg?crop=0.0000%2C0.0572%2C0%2C0&cropmode=percentage&width=600&height=400&quality=80&token=g3bmX6OhH3", 10, 4.5, "beer");

        return Task.CompletedTask;
    }
    
    dynamic CreateProduct(Guid id, string name, string image, decimal price, double stars, string category)
    {
        dynamic product = new ExpandoObject();
        product.Id = id;
        product.UrlId = id.ToString()[..8];
        product.Name = name;
        product.Image = image;
        product.Price = price;
        product.Stars = stars;
        product.Category = category;
        product.InStock = 5;
        return product;
    }
}