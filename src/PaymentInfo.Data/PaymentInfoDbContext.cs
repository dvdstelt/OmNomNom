using Microsoft.EntityFrameworkCore;
using PaymentInfo.Data.Models;

namespace PaymentInfo.Data;

public class PaymentInfoDbContext(DbContextOptions<PaymentInfoDbContext> options) : DbContext(options)
{
    public DbSet<CreditCard> CreditCards => Set<CreditCard>();
    public DbSet<Order> Orders => Set<Order>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // CreditCardId / OrderId picked up as PKs by convention.

        // Order references the credit card chosen at submission time.
        // Restrict on delete so a card row can't disappear from under
        // an existing order.
        modelBuilder.Entity<Order>()
            .HasOne<CreditCard>()
            .WithMany()
            .HasForeignKey(o => o.CreditCardId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
