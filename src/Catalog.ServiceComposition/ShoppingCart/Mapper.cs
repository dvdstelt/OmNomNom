using System.Dynamic;
using Catalog.Data.Models;

namespace Catalog.ServiceComposition.ShoppingCart;

public static class Mapper
{
    public static IDictionary<Guid, dynamic> MapToDictionary(Order order, List<Product> products)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var product in order.Products)
        {
            dynamic vm = new ExpandoObject();
            vm.ProductId = product.ProductId;
            vm.Quantity = product.Quantity;

            var matchingProduct = products.Single(p => p.ProductId == product.ProductId);
            vm.Name = matchingProduct.Name;
            vm.ImageUrl = matchingProduct.ImageUrl;

            productsViewModel[product.ProductId] = vm;
        }

        return productsViewModel;
    }
}