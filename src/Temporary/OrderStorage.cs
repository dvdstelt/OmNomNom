using OmNomNom.Website.ViewModelComposition;

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
        public Address? ShippingAddress { get; set; }
        public Address? BillingAddress { get; set; }

        public decimal TotalPrice => Cart.Items.Sum(item => item.Price);
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

        public static Product[] GetAllProducts() { return [.. products]; }

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

    public class OrderStorage
    {
        private Dictionary<Guid, Order> _orders = new();

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
            _orders.TryGetValue(id, out var order);
            if (order == null)
            {
                order = new Order();
                _orders.Add(id, order);
            }
            return order;
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
