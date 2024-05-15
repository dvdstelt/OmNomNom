using System.Dynamic;
using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using ITOps.Shared;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

public class ProductsHandler(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/products")]
    public async Task Handle(HttpRequest request)
    {
        var products = dbContext.GetAll<Product>();
        var inventory = dbContext.GetAll<InventorySnapshot>();
        var productsModel = Mapper.MapToDictionary(products, inventory.ToList());

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductsLoaded()
        {
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.Products = productsModel.Values.ToList();
    }
}