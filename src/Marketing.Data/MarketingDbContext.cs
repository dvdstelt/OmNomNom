using Marketing.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace Marketing.Data;

public class MarketingDbContext(DbContextOptions<MarketingDbContext> options) : DbContext(options)
{
    public DbSet<Product> Products => Set<Product>();
    public DbSet<OrderActivity> OrderActivity => Set<OrderActivity>();

    // Convention picks up Product.ProductId and OrderActivity.Id as
    // primary keys.
}
