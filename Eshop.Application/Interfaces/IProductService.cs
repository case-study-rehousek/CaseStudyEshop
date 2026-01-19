using Eshop.Application.DTO.Request;
using Eshop.Application.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Application.Interfaces;

public interface IProductService
{
    /// <summary>
    /// Retrieves all products from the system.
    /// </summary>
    /// <returns>A collection of all product response data transfer objects.</returns>
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();

    /// <summary>
    /// Retrieves a specific product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>The product details if found; otherwise, null.</returns>
    Task<ProductResponseDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Creates a new product and assigns a system-calculated display order.
    /// </summary>
    /// <param name="dto">The data required to create a new product.</param>
    /// <returns>The unique identifier of the newly created product.</returns>
    Task<Guid> CreateAsync(CreateProductRequestDto dto);

    /// <summary>
    /// Updates the stock quantity of an existing product.
    /// </summary>
    /// <param name="id">The unique identifier of the product to update.</param>
    /// <param name="dto">The new stock quantity information.</param>
    /// <returns>True if the update was successful; otherwise, false.</returns>
    Task<bool> UpdateStockAsync(Guid id, UpdateStockRequestDto dto);

    /// <summary>
    /// Retrieves a paged list of products for better performance and navigation.
    /// </summary>
    /// <param name="pageNumber">The page number to retrieve (typically starting from 1).</param>
    /// <param name="pageSize">The maximum number of products to return per page.</param>
    /// <returns>A collection of products for the requested page.</returns>
    Task<IEnumerable<ProductResponseDto>> GetPagedProductsAsync(int pageNumber, int pageSize);
}