using System.Dynamic;
using Catalog.ServiceComposition.Workflow;

namespace Catalog.ServiceComposition.Checkout;

// Lean cart-line viewmodel used by the address, payment, and summary
// screens. Carries only ProductId + Quantity; Finance subscribers
// attach Price/Discount via CartSummaryLoaded / SummaryLoaded events,
// and Catalog subscribers attach Name/ImageUrl on /buy/summary.
// The cart and shipping screens need a richer line (Name + ImageUrl)
// and use the heavier Catalog.ShoppingCart.Mapper instead.
public static class Mapper
{
    public static IDictionary<Guid, dynamic> MapToDictionary(CartSlice cart)
    {
        var productsViewModel = new Dictionary<Guid, dynamic>();

        foreach (var line in cart.Items)
        {
            productsViewModel[line.ProductId] = MapToViewModel(line);
        }

        return productsViewModel;
    }

    public static dynamic MapToViewModel(CartLine line)
    {
        dynamic vm = new ExpandoObject();
        vm.ProductId = line.ProductId;
        vm.Quantity = line.Quantity;
        return vm;
    }
}
