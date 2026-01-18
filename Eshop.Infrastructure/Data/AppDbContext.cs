using Eshop.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Eshop.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Zde můžete definovat pravidla, např. přesnost ceny
        modelBuilder.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
    }
}