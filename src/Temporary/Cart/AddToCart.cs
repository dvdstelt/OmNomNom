using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using ServiceComposer.AspNetCore;
using Temporary;

namespace OmNomNom.Website.ViewModelComposition;

public class AddToCart : ICompositionRequestsHandler
{
    private readonly OrderStorage storage;

    public AddToCart(OrderStorage storage)
    {
        this.storage = storage;
    }

    [HttpPost("/cart/addproduct/{orderId}")]
    public async Task Handle(HttpRequest request)
    {
        //// Get cart from session state
        //var value = request.HttpContext.Session.GetString(UserCart.SessionKeyName);
        //var cart = value == null ? new() : JsonSerializer.Deserialize<UserCart>(value);

        //// Bind submitted data to CartItem
        //// var newCartItem = await request.Bind<CartItem>();
        //var newCartItem = new CartItem()
        //{
        //    ProductId = Guid.Parse("cc0981ac-6075-48f5-ad1a-7cae81f2c366"),
        //    Name = "Pizza",
        //    Price = 5m,
        //    Quantity = 2
        //};

        //var existingCartItem = cart?.Items.SingleOrDefault(s => s.ProductId == newCartItem.ProductId);
        //if (existingCartItem != null)
        //    existingCartItem.Quantity += newCartItem.Quantity;
        //else
        //{
        //    cart?.Items.Add(newCartItem);
        //}

        //var serialize = JsonSerializer.Serialize(cart);
        //request.HttpContext.Session.SetString(UserCart.SessionKeyName, serialize);

        var res = await request.Bind<ProductModel>();
        // Using the orderIdString we'll create a proper Guid and try to retrieve a potentially existing order
        var orderId = (res.orderId.ToString() == new Guid().ToString()) ? Guid.NewGuid() : res.orderId;
        storage.AddItem(orderId, res.Detail.Id, res.Detail.Quantity);

        var vm = request.GetComposedResponseModel();
        vm.OrderId = orderId;
    }
}