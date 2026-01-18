using AutoMapper;
using Eshop.Application.DTO.Request;
using Eshop.Application.DTO.Response;
using Eshop.Application.Interfaces;
using Eshop.Domain.Entities;
using Eshop.Domain.Interfaces;


namespace Eshop.Application.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ProductService(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        return product == null ? null : _mapper.Map<ProductResponseDto>(product);
    }

    public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
    {
        var products = await _productRepository.GetAllAsync();
        
        return _mapper.Map<IEnumerable<ProductResponseDto>>(products);
    }

    public async Task<Guid> CreateAsync(CreateProductRequestDto request)
    {
        var product = new Product
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Price = request.Price,
            StockQuantity = request.StockQuantity,
            ImageUrl = request.ImageUrl,
            Description = request.Description
        };

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return product.Id;
    }

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

