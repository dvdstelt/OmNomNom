using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

public class ProductHandler(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/product/{productId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();

        var productIdString = (string)request.HttpContext.GetRouteData().Values["productId"]!;
        var productId = Guid.Parse(productIdString);

        var productCollection = dbContext.Database.GetCollection<Product>();
        var product = productCollection.Query().Where(s => s.ProductId == productId).Single();

        var inventoryCollection = dbContext.Database.GetCollection<InventorySnapshot>();
        var inventoryItem = inventoryCollection.Query().Where(s => s.ProductId == productId).Single();

        var productModel = Mapper.MapToViewModel(product, inventoryItem);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductLoaded()
        {
            ProductId = productId,
            Product = productModel
        });

        vm.Product = productModel;
    }
}