using Eshop.Application.DTO.Request;
using Eshop.Application.DTO.Response;
using Eshop.Application.Interfaces;
using Eshop.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers
{
    /// <summary>
    /// Provides endpoints for managing products, including retrieval, creation, and sorting.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;

        /// <summary>
        /// Initializes a new instance of the ProductsController.
        /// </summary>
        /// <param name="productService">The product service layer.</param>
        public ProductsController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Retrieves a list of all available products.
        /// </summary>
        /// <returns>A list of products.</returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetProducts()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Retrieves a specific product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique ID of the product.</param>
        /// <returns>The requested product details.</returns>
        /// <response code="200">Returns the product details.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetProduct(Guid id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return Ok(product);
        }

        /// <summary>
        /// Creates a new product.
        /// </summary>
        /// <param name="dto">The product creation data.</param>
        /// <returns>The ID of the newly created product.</returns>
        /// <response code="201">Returns the ID of the new product.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<Guid>> CreateProduct([FromBody] CreateProductRequestDto dto)
        {
            var id = await _productService.CreateAsync(dto);
            // Důležité: Přidáváme verzi do parametrů, aby CreatedAtAction správně vygenerovalo URL
            return CreatedAtAction(nameof(GetProduct), new { id, version = "1.0" }, id);
        }

        /// <summary>
        /// Updates the stock quantity of an existing product.
        /// </summary>
        /// <param name="id">The ID of the product to update.</param>
        /// <param name="dto">The new stock quantity data.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Stock updated successfully.</response>
        /// <response code="404">If the product is not found.</response>
        [HttpPatch("{id}/stock")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStock(Guid id, [FromBody] UpdateStockRequestDto dto)
        {
            var success = await _productService.UpdateStockAsync(id, dto);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}
