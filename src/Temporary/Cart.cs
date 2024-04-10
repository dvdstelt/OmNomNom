using System.Dynamic;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;

namespace OmNomNom.Website.ViewModelComposition;

public class Cart : ICompositionRequestsHandler
{
    [HttpGet("/cart/{orderId}")]
    public Task Handle(HttpRequest request)
    {
        var vm = request.GetComposedResponseModel();
        var orderIdString = (string)request.HttpContext.GetRouteData().Values["orderId"]!;

        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially used address 
        var orderId = Guid.Parse("526f0a1d-2900-49ba-9d70-987e9f590b04");

        vm.OrderId = orderId;
        
        var cartItems = new Dictionary<int, dynamic>();
        cartItems[0] = CreateCartItem(Guid.Parse("ff899e9d-4033-48d4-b189-e6ef4a3dc25b"), "Fremont Stout", 10, 3);;
        cartItems[1] = CreateCartItem(Guid.Parse("0b3dcc85-110b-4491-9946-d20c0a51917b"), "Heineken", 0.5m, 24);;
        vm.CartItems = cartItems.Values.ToList();
        vm.TotalPrice = 42.0m;

        return Task.CompletedTask;
    }
    
    dynamic CreateCartItem(Guid id, string name, decimal price, int quantity)
    {
        dynamic product = new ExpandoObject();
        product.Id = id;
        product.Name = name;
        product.Price = price;
        product.Quantity = quantity;
        return product;
    }
}