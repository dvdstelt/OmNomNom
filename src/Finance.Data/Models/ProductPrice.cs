namespace Finance.Data.Models;

// An immutable, point-in-time price for a product. A price change never
// updates a row; it appends a new one with a later ValidFrom. The current
// price for a product is the row with the highest ValidFrom. Orders lock
// in the PriceId they were placed against, so the amount billed never
// shifts when a newer price appears.
public class ProductPrice : IPriced
{
    public Guid PriceId { get; set; }
    public Guid ProductId { get; set; }
    public decimal Price { get; set; }
    public decimal Discount { get; set; }
    public DateTimeOffset ValidFrom { get; set; }
}
