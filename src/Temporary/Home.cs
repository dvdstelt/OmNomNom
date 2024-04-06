using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Home : ICompositionRequestsHandler
{
    [HttpGet("/")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        vm.MyName = "Dennis";

        var products = new Dictionary<int, dynamic>();
        products[0] = CreateProduct(Guid.Parse("09390028-3d57-4c34-977d-9eb78f146618"), "Pizza", "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a3/Eq_it-na_pizza-margherita_sep2005_sml.jpg/640px-Eq_it-na_pizza-margherita_sep2005_sml.jpg", 5, 3.5);
        products[1] = CreateProduct(Guid.Parse("ff899e9d-4033-48d4-b189-e6ef4a3dc25b"), "Beer", "https://www.surprisefactory.nl/media/48245/zelf-bier-brouwen.jpg?crop=0.0000%2C0.0572%2C0%2C0&cropmode=percentage&width=600&height=400&quality=80&token=g3bmX6OhH3", 3, 4.5);;

        vm.Products = products.Values.ToList();

        return Task.CompletedTask;
    }

    dynamic CreateProduct(Guid id, string name, string image, decimal price, double stars)
    {
        dynamic product = new ExpandoObject();
        product.Id = id.ToString()[8..];
        product.Name = name;
        product.Image = image;
        product.Price = price;
        product.Stars = stars;
        return product;
    }
}

