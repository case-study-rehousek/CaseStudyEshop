using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Eshop.Infrastructure.Repositories;

/// <summary>
/// Entity Framework Core implementation of the product repository for persistent storage.
/// </summary>
public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductRepository"/> class.
    /// </summary>
    /// <param name="context">The database context used for data operations.</param>
    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retrieves all products from the database, ordered by the display order property.
    /// </summary>
    /// <returns>A collection of all products from the database.</returns>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .OrderBy(p => p.Order)
            .ToListAsync();
    }

    /// <summary>
    /// Retrieves a paginated subset of products from the database using Skip and Take.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The maximum number of items per page.</param>
    /// <returns>A paginated collection of products from the database.</returns>
    public async Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.Products
            .OrderBy(p => p.Order)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    /// <summary>
    /// Finds a product by its primary key.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The product entity if found; otherwise, null.</returns>
    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    /// <summary>
    /// Asynchronously adds a new product entity to the database context.
    /// </summary>
    /// <param name="product">The product entity to persist.</param>
    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    /// <summary>
    /// Sets the entity state to modified for an existing product.
    /// </summary>
    /// <param name="product">The product entity with updated data.</param>
    public async Task UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    /// <summary>
    /// Saves all changes tracked by the database context to the database.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Calculates the maximum order value currently stored in the Products table.
    /// </summary>
    /// <returns>The highest order number or null if the table is empty.</returns>
    public async Task<int?> GetMaxOrder()
    {
        // Check if there are any products to avoid exception when calling MaxAsync on an empty set
        if (!await _context.Products.AnyAsync())
        {
            return null;
        }

        return await _context.Products.MaxAsync(p => p.Order);
    }
}
