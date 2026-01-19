using AutoMapper;
using Eshop.Application.DTO.Request;
using Eshop.Application.DTO.Response;
using Eshop.Application.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;
using System.Linq;

namespace Eshop.Application.Services;

/// <summary>
/// Service responsible for managing product-related business logic.
/// </summary>
public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="ProductService"/> class.
    /// </summary>
    /// <param name="productRepository">The repository for data access.</param>
    /// <param name="mapper">The AutoMapper instance for object mapping.</param>
    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves a product by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the product.</param>
    /// <returns>A product response DTO if found; otherwise, null.</returns>
    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : _mapper.Map<ProductResponseDto>(product);
    }

    /// <summary>
    /// Retrieves all products available in the system.
    /// </summary>
    /// <returns>A collection of product response DTOs.</returns>
    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();

        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    /// <summary>
    /// Retrieves a paginated list of products based on page number and size.
    /// </summary>
    /// <param name="pageNumber">The number of the page to retrieve.</param>
    /// <param name="pageSize">The number of products per page.</param>
    /// <returns>A paginated collection of product response DTOs.</returns>
    public async Task<IEnumerable<ProductResponseDto>> GetPagedProductsAsync(int pageNumber, int pageSize)
    {
        var products = await _productRepository.GetPagedAsync(pageNumber, pageSize);
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    /// <summary>
    /// Creates a new product and automatically assigns the next display order value.
    /// </summary>
    /// <param name="request">The product creation request data.</param>
    /// <returns>The unique identifier of the newly created product.</returns>
    public async Task<Guid> CreateAsync(CreateProductRequestDto request)
    {
        // Get the current highest order value from the repository
        var maxOrder = await _productRepository.GetMaxOrder();

        // Calculate next order (increment max or start at 1 if no products exist)
        int nextOrder = (maxOrder ?? 0) + 1;

        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ImageUrl = request.ImageUrl,
            Description = request.Description,
            Order = nextOrder
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return product.Id;
    }

    /// <summary>
    /// Updates the stock quantity for a specific product.
    /// </summary>
    /// <param name="productId">The unique identifier of the product to update.</param>
    /// <param name="request">The data containing the new stock quantity.</param>
    /// <returns>True if the update was successful; false if the product was not found.</returns>
    /// <exception cref="ArgumentException">Thrown when the new quantity is less than zero.</exception>
    public async Task<bool> UpdateStockAsync(Guid productId, UpdateStockRequestDto request)
    {
        if (request.NewQuantity < 0)
        {
            throw new ArgumentException("Quantity must be zero or greater.");
        }

        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null) return false;

        product.StockQuantity = request.NewQuantity;

        await _productRepository.UpdateAsync(product);
        await _productRepository.SaveChangesAsync();

        return true;
    }
}