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
        var productCollection = dbContext.Database.GetCollection<Product>();
        var products = productCollection.Query().ToList();

        var inventoryCollection = dbContext.Database.GetCollection<InventorySnapshot>();
        var inventory = inventoryCollection.Query().ToList();

        var productsModel = Mapper.MapToDictionary(products, inventory);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductsLoaded()
        {
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.Products = productsModel.Values.ToList();
    }
}