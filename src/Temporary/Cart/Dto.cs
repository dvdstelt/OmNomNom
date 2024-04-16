using Microsoft.AspNetCore.Mvc;

namespace OmNomNom.Website.ViewModelComposition;

class RequestModel
{
    [FromRoute] public Guid orderId { get; set; }
    [FromBody] public Dictionary<string, Dto> Body { get; set; }
}

class Dto
{
    public string Street { get; set; }
}

public class UserCart
{
    public const string SessionKeyName = "UserCart";

    public Guid OrderId { get; set; }
    public List<CartItem> Items { get; set; } = new();
}

public class CartItem
{
    public int Quantity { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public string Name { get; set; }
}