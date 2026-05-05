using Catalog.Data;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Products;

public class ProductsHandler(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/products")]
    public async Task Handle(HttpRequest request)
    {
        var products = await dbContext.Products.ToListAsync(request.HttpContext.RequestAborted);
        var inventory = await dbContext.InventorySnapshots.ToListAsync(request.HttpContext.RequestAborted);
        var productsModel = Mapper.MapToDictionary(products, inventory);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductsLoaded
        {
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.Products = productsModel.Values.ToList();
    }
}
