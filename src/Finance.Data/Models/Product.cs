namespace Finance.Data.Models;

public class Product
{
    public Guid ProductId { get; set; }

    // The product's price history. The current price is the entry with
    // the highest ValidFrom; see ProductPriceQueries.
    public List<ProductPrice> Prices { get; set; } = [];
}
