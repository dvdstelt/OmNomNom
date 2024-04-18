using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;
using Temporary;

namespace OmNomNom.Website.ViewModelComposition;

public class ProductsHandler : ICompositionRequestsHandler
{
    [HttpGet("/products")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        vm.Products = Products.GetAllProducts();

        return Task.CompletedTask;
    }
}

