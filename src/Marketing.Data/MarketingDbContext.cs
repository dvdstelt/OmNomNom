using Marketing.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Data;

public class MarketingDbContext(DbContextOptions<MarketingDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();

    // Convention picks up Product.ProductId as the primary key.
}
