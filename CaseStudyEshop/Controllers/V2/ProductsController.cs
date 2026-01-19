using Asp.Versioning;
using Eshop.Application.DTO.Request;
using Eshop.Application.DTO.Response;
using Eshop.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eshop.API.Controllers.V2
{
    /// <summary>
    /// Provides endpoints for managing products, including retrieval, creation, and sorting.
    /// </summary>
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
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
        /// Retrieves a paged list of available products (Version 2.0).
        /// </summary>
        /// <param name="pageNumber">The number of the page to retrieve. Minimum is 1.</param>
        /// <param name="pageSize">Number of items per page. Between 1 and 100.</param>
        /// <response code="200">Returns the paged list of products.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetPagedProducts([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Normalizace parametrů
            pageNumber = pageNumber < 1 ? 1 : pageNumber;
            pageSize = (pageSize < 1 || pageSize > 100) ? 10 : pageSize;

            var products = await _productService.GetPagedProductsAsync(pageNumber, pageSize);

            // Volitelné: Přidání informace o počtu prvků do headeru
            // Response.Headers.Add("X-Total-Count", products.Count().ToString());

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
            return CreatedAtAction(nameof(GetProduct), new { id, version = "2.0" }, id);
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
