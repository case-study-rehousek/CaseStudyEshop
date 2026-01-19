using Eshop.Domain.Entities;

namespace Eshop.Domain.Interfaces;

/// <summary>
/// Defines the data access operations for the Product entity.
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Retrieves all products from the database.
    /// </summary>
    /// <returns>A collection of all products.</returns>
    Task<IEnumerable<Product>> GetAllAsync();

    /// <summary>
    /// Retrieves a subset of products for pagination.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A collection of products for the specified page.</returns>
    Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize);

    /// <summary>
    /// Finds a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The product entity if found; otherwise, null.</returns>
    Task<Product?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adds a new product entity to the data context.
    /// </summary>
    /// <param name="product">The product entity to add.</param>
    Task AddAsync(Product product);

    /// <summary>
    /// Updates an existing product entity in the data context.
    /// </summary>
    /// <param name="product">The product entity with updated values.</param>
    Task UpdateAsync(Product product);

    /// <summary>
    /// Persists all changes made in the data context to the database.
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    /// Retrieves the maximum value of the Order property currently stored in the database.
    /// </summary>
    /// <returns>The highest order number if products exist; otherwise, null.</returns>
    Task<int?> GetMaxOrder();
}
