using Eshop.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Infrastructure.Mocks;

/// <summary>
/// Provides static mock data for products used during development and testing.
/// </summary>
public static class ProductMockData
{
    /// <summary>
    /// Generates a predefined list of mock product entities.
    /// </summary>
    /// <returns>A list of initialized <see cref="Product"/> objects.</returns>
    public static List<Product> GetMockProducts()
    {
        return new List<Product>
        {
            new Product { Id = Guid.Parse("00000000-0000-0000-0000-000000000001"), Name = "Mock Laptop", Price = 25000, Order = 1, StockQuantity = 5 },
            new Product { Id = Guid.Parse("00000000-0000-0000-0000-000000000002"), Name = "Mock Mouse", Price = 500, Order = 2, StockQuantity = 50 },
            new Product { Id = Guid.Parse("00000000-0000-0000-0000-000000000003"), Name = "Mock Keyboard", Price = 1200, Order = 3, StockQuantity = 20 }
        };
    }
}
