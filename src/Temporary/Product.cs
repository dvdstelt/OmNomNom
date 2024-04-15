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
        switch (productId)
        {
            case "09390028-3d57-4c34-977d-9eb78f146618":
                vm.Product = CreateProduct(Guid.Parse(productId), "Pizza", "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a3/Eq_it-na_pizza-margherita_sep2005_sml.jpg/640px-Eq_it-na_pizza-margherita_sep2005_sml.jpg", 5, 5, 3.5, 7, "food");
                break;
            case "ff899e9d-4033-48d4-b189-e6ef4a3dc25b":
                vm.Product = CreateProduct(Guid.Parse(productId), "Fremont Stout", "https://www.surprisefactory.nl/media/48245/zelf-bier-brouwen.jpg?crop=0.0000%2C0.0572%2C0%2C0&cropmode=percentage&width=600&height=400&quality=80&token=g3bmX6OhH3", 10, 83, 4.5, 42, "beer");
                break;
            case "0b3dcc85-110b-4491-9946-d20c0a51917b":
                vm.Product = CreateProduct(Guid.Parse(productId), "Heineken", "https://media.danmurphys.com.au/dmo/product/804969-1.png?impolicy=PROD_LG", 0.5m, 1843, 4.5, 212, "beer");
                break;
        }
        //vm.Product = CreateProduct(Guid.Parse("ff899e9d-4033-48d4-b189-e6ef4a3dc25b"), "Fremont Stout", "https://www.surprisefactory.nl/media/48245/zelf-bier-brouwen.jpg?crop=0.0000%2C0.0572%2C0%2C0&cropmode=percentage&width=600&height=400&quality=80&token=g3bmX6OhH3", 10, 4.5, 42, "beer");

        return Task.CompletedTask;
    }
    
    dynamic CreateProduct(Guid id, string name, string image, decimal price, double stockCount, double stars, int reviewCount, string category)
    {
        dynamic product = new ExpandoObject();
        product.Id = id;
        product.UrlId = id.ToString()[..8];
        product.Name = name;
        product.Image = image;
        product.Price = price;
        product.Stars = stars;
        product.ReviewCount = reviewCount;
        product.Category = category;
        product.InStock = stockCount;
        return product;
    }
}