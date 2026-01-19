using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;
using Eshop.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;

namespace Eshop.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .OrderBy(p => p.Order)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetPagedAsync(int pageNumber, int pageSize)
    {
        return await _context.Products
            .OrderBy(p => p.Order)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Product?> GetByIdAsync(Guid id)
    {
        return await _context.Products.FindAsync(id);
    }

    public async Task AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await Task.CompletedTask;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    public async Task<int?> GetMaxOrder()
    {
        if (!await _context.Products.AnyAsync())
        {
            return null;
        }

        return await _context.Products.MaxAsync(p => p.Order);
    }
}
