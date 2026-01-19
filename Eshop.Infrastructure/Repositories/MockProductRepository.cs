using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;
using Eshop.Infrastructure.Mocks;


namespace Eshop.Infrastructure.Repositories;


/// <summary>
/// In-memory implementation of the product repository for testing and development purposes.
/// </summary>
public class MockProductRepository : IProductRepository
{
    private readonly List<Product> _products;

    /// <summary>
    /// Initializes a new instance of the <see cref="MockProductRepository"/> class with seed data.
    /// </summary>
    public MockProductRepository()
    {
        // Load initial data from our static MockData class
        _products = ProductMockData.GetMockProducts();
    }

    /// <summary>
    /// Retrieves all products from the in-memory list, ordered by their display order.
    /// </summary>
    /// <returns>A collection of all products.</returns>
    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await Task.FromResult(_products.OrderBy(p => p.Order));
    }

    /// <summary>
    /// Retrieves a paginated subset of products from the in-memory list.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A paginated collection of products.</returns>
    public async Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var pagedProducts = _products
            .OrderBy(p => p.Order)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return await Task.FromResult(pagedProducts);
    }

    /// <summary>
    /// Finds a product in the in-memory list by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The product entity if found; otherwise, null.</returns>
    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    }

    /// <summary>
    /// Adds a new product to the in-memory list. Generates a new ID if not provided.
    /// </summary>
    /// <param name="product">The product entity to add.</param>
    public async Task AddAsync(Product product)
    {
        if (product.Id == Guid.Empty) product.Id = Guid.NewGuid();
        _products.Add(product);
        await Task.CompletedTask;
    }

    /// <summary>
    /// Updates an existing product in the in-memory list.
    /// </summary>
    /// <param name="product">The product entity containing updated values.</param>
    public async Task UpdateAsync(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index != -1)
        {
            _products[index] = product;
        }
        await Task.CompletedTask;
    }

    /// <summary>
    /// Simulates persisting changes. In this mock implementation, it performs no physical action.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        // No physical persistence required for mock data
        await Task.CompletedTask;
    }

    /// <summary>
    /// Returns the maximum order value currently present in the in-memory list.
    /// </summary>
    /// <returns>The highest order number or null if the list is empty.</returns>
    public async Task<int?> GetMaxOrder()
    {
        if (!_products.Any())
        {
            return null;
        }
        return await Task.FromResult(_products.Max(p => p.Order));
    }
}