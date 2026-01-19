using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Eshop.Application.DTO.Request;


/// <summary>
/// Data transfer object used for creating a new product.
/// </summary>
public class CreateProductRequestDto
{
    /// <summary>
    /// The unique name of the product. This field is required.
    /// </summary>
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    public required string Name { get; set; }

    /// <summary>
    /// The absolute URL to the product's image. This field is required.
    /// </summary>
    [Required(ErrorMessage = "Image URL is required.")]
    [Url(ErrorMessage = "Invalid URL format.")]
    public required string ImageUrl { get; set; }

    /// <summary>
    /// The unit price of the product. Must be between 0.01 and 100,000.
    /// </summary>
    [Range(0.01, 100000, ErrorMessage = "Price must be between 0.01 and 100,000.")]
    public decimal Price { get; set; } = 0;

    /// <summary>
    /// An optional detailed description of the product.
    /// </summary>
    [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
    public string? Description { get; set; }

    /// <summary>
    /// The initial amount of items in stock. Must be a non-negative number.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    public int StockQuantity { get; set; } = 0;
}
