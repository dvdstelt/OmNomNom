using Microsoft.AspNetCore.Mvc;

namespace OmNomNom.Website.ViewModelComposition;

class AddressModel
{
    [FromRoute] public Guid orderId { get; set; }
    [FromBody] public AddressModelBody Body { get; set; }
}

class AddressModelBody
{
    public string FullName { get; set; }
    public Address ShippingAddress { get; set; }
    public Address BillingAddress { get; set; }
}

public class Address
{
    public string Street { get; set; } = string.Empty;
    public string ZipCode { get; set; } = string.Empty;
    public string Town { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;

    public override bool Equals(object? obj)
    {
        return obj is Address address &&
               Street == address.Street &&
               ZipCode == address.ZipCode &&
               Town == address.Town &&
               Country == address.Country;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Street, ZipCode, Town, Country);
    }
}

public class ProductModel
{
    [FromRoute] public Guid orderId { get; set; }
    [FromBody] public ProductModelBody Detail { get; set; }
}

public class ProductModelBody
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
}

public class Product
{
    public Guid Id { get; internal set; }
    public string UrlId { get; internal set; }
    public string Name { get; internal set; }
    public string Image { get; internal set; }
    public decimal Price { get; internal set; }
    public double Stars { get; internal set; }
    public string Category { get; internal set; }
    public int InStock { get; internal set; }
    public int ReviewCount { get; internal set; }
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

public class DeliveryOption
{
    public Guid Id { get; internal set; }
    public string Name { get; internal set; }
    public string Description { get; internal set; }
    public decimal Price { get; internal set; }
}

public class ShippingModel
{
    [FromRoute] public Guid orderId { get; set; }
    [FromBody] public ShippingModelBody Detail { get; set; }
}

public class ShippingModelBody
{
    public Guid DeliveryOptionId { get; set; }
}