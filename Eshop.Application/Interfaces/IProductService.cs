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
    Task<IEnumerable<ProductResponseDto>> GetAllAsync();
    Task<ProductResponseDto?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(CreateProductRequestDto dto);
    Task<bool> UpdateStockAsync(Guid id, UpdateStockRequestDto dto);

    Task<IEnumerable<ProductResponseDto>> GetPagedProductsAsync(int pageNumber, int pageSize);
}