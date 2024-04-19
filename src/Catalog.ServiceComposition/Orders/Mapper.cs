using System.Dynamic;
using System.Runtime.InteropServices;
using Catalog.Data.Models;

namespace Catalog.ServiceComposition.Orders;

public static class Mapper
{
    public static IDictionary<Guid, dynamic> MapToDictionary(Order order, List<Product> products)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var product in order.Products)
        {
            dynamic vm = new ExpandoObject();
            vm.ProductId = product.Value.ProductId;
            vm.Quantity = product.Value.Quantity;

            var matchingProduct = products.Single(p => p.ProductId == product.Value.ProductId);
            vm.Name = matchingProduct.Name;
            vm.ImageUrl = matchingProduct.ImageUrl;

            productsViewModel[product.Value.ProductId] = vm;
        }

        return productsViewModel;
    }
}