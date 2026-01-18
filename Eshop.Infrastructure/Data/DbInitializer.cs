using Eshop.Domain.Entities;

namespace Eshop.Infrastructure.Data;

public static class DbInitializer
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Products.Any()) return; // Databáze už obsahuje data

        var products = new List<Product>();
        for (int i = 1; i <= 100; i++)
        {
            products.Add(new Product
            {
                Id = Guid.NewGuid(),
                Name = $"Produkt {i}",
                Description = $"Popis pro produkt číslo {i}",
                Price = 100 + i,
                ImageUrl = $"https://picsum.photos/seed/{i}/200",
                StockQuantity = i,
                Order = i
            });
        }

        context.Products.AddRange(products);
        context.SaveChanges();
    }
}