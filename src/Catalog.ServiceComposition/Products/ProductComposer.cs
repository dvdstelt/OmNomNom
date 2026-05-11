using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

public class ProductComposer(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/product/{productId}")]
    public async Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();

        var productIdString = (string)request.HttpContext.GetRouteData().Values["productId"]!;
        var productId = Guid.Parse(productIdString);

        var ct = request.HttpContext.RequestAborted;
        var row = await (
            from p in dbContext.Products
            join s in dbContext.InventorySnapshots on p.ProductId equals s.ProductId
            where p.ProductId == productId
            select new { Product = p, Inventory = s }).SingleAsync(ct);

        var productModel = Mapper.MapToViewModel(row.Product, row.Inventory);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductLoaded
        {
            ProductId = productId,
            Product = productModel
        });

        vm.Product = productModel;
    }
}
