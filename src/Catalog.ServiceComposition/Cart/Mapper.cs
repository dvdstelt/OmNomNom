using System.Dynamic;
using Catalog.Data.Models;
using Catalog.ServiceComposition.Workflow;

namespace Catalog.ServiceComposition.Cart;

public static class Mapper
{
    public static IDictionary<Guid, dynamic> MapToDictionary(
        CartSlice cart,
        IReadOnlyDictionary<Guid, (Product Product, int InStock)> productLookup)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var line in cart.Items)
        {
            if (!productLookup.TryGetValue(line.ProductId, out var row))
            {
                continue;
            }
            productsViewModel[line.ProductId] = MapToViewModel(line, row.Product, row.InStock);
        }

        return productsViewModel;
    }

    public static dynamic MapToViewModel(CartLine line, Product product, int inStock)
    {
        dynamic vm = new ExpandoObject();
        vm.ProductId = line.ProductId;
        vm.Quantity = line.Quantity;
        vm.Name = product.Name;
        vm.ImageUrl = product.ImageUrl;
        // Live ledger count so the cart UI can cap its + button. Cart
        // composition can race with stock changes from other customers
        // checking out; the OrderPlacedHandler is still the authority
        // when an order is finalized.
        vm.InStock = inStock;
        return vm;
    }
}
