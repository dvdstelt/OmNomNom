using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

[CompositionHandler]
public class ProductComposer(CatalogDbContext dbContext, IHttpContextAccessor http)
{
    [HttpGet("/product/{productId}")]
    public async Task Handle(Guid productId)
    {
        var request = http.HttpContext!.Request;
        var vm = request.GetComposedResponseModel();
        var ct = request.HttpContext.RequestAborted;

        var row = await dbContext.Products
            .Where(p => p.ProductId == productId)
            .Select(p => new
            {
                Product = p,
                InStock = dbContext.InventoryDeltas
                    .Where(d => d.ProductId == p.ProductId)
                    .Sum(d => (int?)d.Delta) ?? 0
            })
            .SingleAsync(ct);

        var productModel = Mapper.MapToViewModel(row.Product, row.InStock);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductLoaded
        {
            ProductId = productId,
            Product = productModel
        });

        vm.Product = productModel;
    }
}
