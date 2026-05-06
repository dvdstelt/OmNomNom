namespace Marketing.Data.Models;

public class Product
{
    public Guid ProductId { get; set; }

    // Quality signals (Untappd-derived once URLs are available; hand-picked for now).
    public double Rating { get; set; }
    public int RatingCount { get; set; }

    // Behavioural signals derived from OrderPlaced events.
    // OrderCount is denormalised, bumped on each event.
    // Trending is denormalised too: bumped on each event AND
    // periodically recomputed by TrendingRecomputeService so old
    // events drop out of the 30-day window.
    public int OrderCount { get; set; }
    public int Trending { get; set; }
}