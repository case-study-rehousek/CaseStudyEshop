using Eshop.Application.DTO.Request;
using Eshop.Application.DTO.Response;
using Eshop.Application.Interfaces;
using Eshop.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductRequestDto dto)
        {
            var id = await _productService.CreateAsync(dto);            
            return CreatedAtAction(nameof(GetProduct), new { id = id, version = "1.0" }, id);
        }

        [HttpPatch("{id}/stock")]
        public async Task<IActionResult> UpdateStock(Guid id, UpdateStockRequestDto dto)
        {
            var success = await _productService.UpdateStockAsync(id, dto);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
