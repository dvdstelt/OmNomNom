namespace ITOps.Shared.Integration;

public interface IProvideData
{
}

public interface IProvideOrderData : IProvideData
{
    Order GetOrderInfo(Guid orderId);
}

public class Order
{
    public Address BillingAddress { get; set; }

    public decimal TotalPrice { get; set; }

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public decimal Discount { get; set; }
    }

    public class Address
    {
        public string FullName { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string Town { get; set; }
        public string Country { get; set; }
    }
}