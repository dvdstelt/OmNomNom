using OmNomNom.Website.ViewModelComposition;
using System.Collections.Concurrent;

namespace Temporary
{
    public class Order
    {
        public Order()
        {
            Id = Guid.NewGuid();
            Cart = new();
        }

        public Guid Id { get; set; }
        public UserCart Cart { get; set; }

        public string FullName { get; set; } = string.Empty;
        public Address? ShippingAddress { get; set; }
        public Address? BillingAddress { get; set; }

        public Guid? DeliveryOptionId { get; set; }

        public decimal TotalCartPrice => Cart.Items.Sum(item => item.Price * item.Quantity);
        public decimal TotalBeforeTaxPrice => TotalCartPrice + (DeliveryOptionId.HasValue ? DeliveryOptions.GetDeliveryOptionById(DeliveryOptionId.Value).Price : 0m);
        public decimal TaxPercent { get; set; } = 0;
        public decimal TaxPrice => TotalBeforeTaxPrice * TaxPercent;
        public decimal TotalPrice => TotalBeforeTaxPrice + TaxPrice;
    }

    public static class Products
    {
        private static readonly Product[] products = [
            CreateProduct(Guid.Parse("09390028-3d57-4c34-977d-9eb78f146618"), "Pizza", "https://upload.wikimedia.org/wikipedia/commons/thumb/a/a3/Eq_it-na_pizza-margherita_sep2005_sml.jpg/640px-Eq_it-na_pizza-margherita_sep2005_sml.jpg", 5, 5, 3.7, 7, "food"),
            CreateProduct(Guid.Parse("ff899e9d-4033-48d4-b189-e6ef4a3dc25b"), "Fremont Stout", "https://www.surprisefactory.nl/media/48245/zelf-bier-brouwen.jpg?crop=0.0000%2C0.0572%2C0%2C0&cropmode=percentage&width=600&height=400&quality=80&token=g3bmX6OhH3", 10, 83, 4.5, 42, "beer"),
            CreateProduct(Guid.Parse("0b3dcc85-110b-4491-9946-d20c0a51917b"), "Heineken", "https://media.danmurphys.com.au/dmo/product/804969-1.png?impolicy=PROD_LG", 0.5m, 1843, 4.8, 212, "beer")
        ];

        public static Product GetProductById(Guid id)
        {
            return products.Single(p => p.Id == id);
        }

        public static Product[] GetAllProducts() => [.. products];

        private static Product CreateProduct(Guid id, string name, string image, decimal price, int stockCount, double stars, int reviewCount, string category)
        {
            var product = new Product
            {
                Id = id,
                UrlId = id.ToString()[..8],
                Name = name,
                Image = image,
                Price = price,
                Stars = stars,
                ReviewCount = reviewCount,
                Category = category,
                InStock = stockCount
            };
            return product;
        }
    }

    public static class DeliveryOptions
    {
        private static readonly DeliveryOption[] deliveryOptions = [
            CreateDeliveryOption(Guid.Parse("071f3894-762e-4fb2-b55c-8c65f2641f5b"), "Standard shipping", "7-10 business days", 7.98m),
            CreateDeliveryOption(Guid.Parse("155e3818-ff4d-43f1-9000-6d6bb2d2f736"), "Expedited shipping", "2-5 business days", 20.98m),
            CreateDeliveryOption(Guid.Parse("3835fb8d-be88-4d39-9eb8-043cac3c9695"), "Priority shipping", "Next business day", 49.98m)
        ];

        public static DeliveryOption GetDeliveryOptionById(Guid id)
        {
            return deliveryOptions.Single(p => p.Id == id);
        }

        public static DeliveryOption[] GetAllDeliveryOptions() => [.. deliveryOptions];

        private static DeliveryOption CreateDeliveryOption(Guid id, string name, string description, decimal price)
        {
            return new DeliveryOption
            {
                Id = id,
                Name = name,
                Description = description,
                Price = price
            };
        }
    }

    public class OrderStorage
    {
        private ConcurrentDictionary<Guid, Order> _orders = new();

        public Guid AddItem(Guid? id, CartItem item)
        {
            var order = GetOrder(ref id);

            order.Cart.Items.Add(item);
            return id.Value;
        }

        public Guid AddItem(Guid? id, Guid productId, int quantity)
        {
            var order = GetOrder(ref id);
            var existing = order.Cart.Items.Find(item => item.ProductId == productId);
            if (existing != null)
                existing.Quantity += quantity;
            else
            {
                var item = Products.GetProductById(productId);
                order.Cart.Items.Add(new CartItem { ProductId = productId, Quantity = quantity, Name = item.Name, Price = item.Price });
            }

            return id.Value;
        }

        public Order GetOrder(Guid id)
        {
            return _orders.GetOrAdd(id, new Order());
        }

        private Order GetOrder(ref Guid? id)
        {
            if (!id.HasValue)
            {
                id = Guid.NewGuid();
            }
            return GetOrder(id.Value);
        }
    }
}
