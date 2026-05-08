namespace Marketing.Data.Models;

// Append-only log of order activity, used as the source of truth for
// the Trending recompute. One row per (placed order, ordered product)
// line item. Marketing keeps this rather than reading from Catalog so
// the service boundary stays clean.
public class OrderActivity
{
    public long Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime OccurredAt { get; set; }
}
