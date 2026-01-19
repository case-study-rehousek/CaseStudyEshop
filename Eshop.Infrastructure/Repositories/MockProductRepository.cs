using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;
using Eshop.Infrastructure.Mocks;


namespace Eshop.Infrastructure.Repositories;

public class MockProductRepository : IProductRepository
{
    private readonly List<Product> _products;

    public MockProductRepository()
    {
        // Načteme data z naší statické třídy MockData
        _products = ProductMockData.GetMockProducts();
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await Task.FromResult(_products.OrderBy(p => p.Order));
    }

    public async Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize)
    {
        var pagedProducts = _products
            .OrderBy(p => p.Order)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        return await Task.FromResult(pagedProducts);
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await Task.FromResult(_products.FirstOrDefault(p => p.Id == id));
    }

    public async Task AddAsync(Product product)
    {
        if (product.Id == Guid.Empty) product.Id = Guid.NewGuid();
        _products.Add(product);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(Product product)
    {
        var index = _products.FindIndex(p => p.Id == product.Id);
        if (index != -1)
        {
            _products[index] = product;
        }
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        // Nic se fyzicky neukládá, jen kvůli rozhraní        
        await Task.CompletedTask;
    }

    public async Task<int?> GetMaxOrder()
    {
        if (!_products.Any())
        {
            return null;
        }
        return await Task.FromResult(_products.Max(p => p.Order));
    }
}