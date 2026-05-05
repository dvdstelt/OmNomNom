using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
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

        var ct = request.HttpContext.RequestAborted;
        var product = await dbContext.Products.SingleAsync(s => s.ProductId == productId, ct);
        var inventoryItem = await dbContext.InventorySnapshots.SingleAsync(s => s.ProductId == productId, ct);

        var productModel = Mapper.MapToViewModel(product, inventoryItem);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductLoaded
        {
            ProductId = productId,
            Product = productModel
        });

        vm.Product = productModel;
    }
}
