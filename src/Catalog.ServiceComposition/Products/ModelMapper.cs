using System.Dynamic;
using Catalog.Data.Models;

namespace Catalog.ServiceComposition.Products;

public class ModelMapper
{
    public static IDictionary<Guid, dynamic> MapToDictionary(IEnumerable<Product> products, List<InventorySnapshot> inventory)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var product in products)
        {
            var inventoryItem = inventory.Single(p => p.ProductId == product.ProductId);
            var vm = MapToViewModel(product, inventoryItem);

            productsViewModel[product.ProductId] = vm;
        }

        return productsViewModel;
    }

    public static dynamic MapToViewModel(Product product, InventorySnapshot inventoryItem)
    {
        dynamic vm = new ExpandoObject();
        vm.ProductId = product.ProductId;
        vm.Name = product.Name;
        vm.Description = product.Description;
        vm.ImageUrl = product.ImageUrl;
        vm.Category = product.Category;
        vm.InStock = inventoryItem.EstimatedInStock;
        return vm;
    }
}