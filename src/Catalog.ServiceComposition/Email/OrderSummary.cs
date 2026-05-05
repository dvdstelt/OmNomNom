using System.Dynamic;
using Catalog.Data;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Events;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ServiceComposer.AspNetCore;

namespace Catalog.ServiceComposition.Email;

public class OrderSummary(CatalogDbContext dbContext) : ICompositionRequestsHandler
{
    [HttpGet("/email/summary/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        var orderData = await request.Bind<OrderData>();
        var ct = request.HttpContext.RequestAborted;

        var order = await dbContext.Orders
            .Include(o => o.Products)
            .SingleAsync(s => s.OrderId == orderData.OrderId, ct);

        var productIds = order.Products.Select(s => s.ProductId).ToList();
        var products = await dbContext.Products
            .Where(s => productIds.Contains(s.ProductId))
            .ToListAsync(ct);

        var productsModel = MapToDictionary(order.Products, products);

        var context = request.GetCompositionContext();
        await context.RaiseEvent(new ProductsLoaded
        {
            Products = productsModel
        });

        var vm = request.GetComposedResponseModel();
        vm.Products = productsModel.Values.ToList();
        vm.OrderId = orderData.OrderId;
    }

    public IDictionary<Guid, dynamic> MapToDictionary(IEnumerable<OrderItem> orderedProducts, List<Product> products)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var orderedProduct in orderedProducts)
        {
            var product = products.Single(s => s.ProductId == orderedProduct.ProductId);
            var vm = MapToViewModel(orderedProduct, product);

            productsViewModel[orderedProduct.ProductId] = vm;
        }

        return productsViewModel;
    }

    private dynamic MapToViewModel(OrderItem orderedProduct, Product product)
    {
        dynamic vm = new ExpandoObject();
        vm.ProductId = orderedProduct.ProductId;
        vm.Name = product.Name;
        vm.Description = product.Description;
        vm.ImageUrl = product.ImageUrl;
        vm.Quantity = orderedProduct.Quantity;
        return vm;
    }

    class OrderData
    {
        [FromRoute] public Guid OrderId { get; set; }
    }
}
