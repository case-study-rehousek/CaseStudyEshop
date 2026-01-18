using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Application.DTO.Response;

/// <summary>
/// Data transfer object representing a product's details for API responses.
/// </summary>
public class ProductResponseDto
{
    /// <summary>
    /// The unique identifier of the product.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The display name of the product.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// A detailed description of the product's features and specifications. 
    /// Can be null if no description is provided.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The current unit price of the product.
    /// </summary>
    public decimal Price { get; set; }

    /// <summary>
    /// The absolute URL to the product's primary image.
    /// </summary>
    public string ImageUrl { get; set; } = string.Empty;

    /// <summary>
    /// The current number of items available in the inventory.
    /// </summary>
    public int StockQuantity { get; set; }

    /// <summary>
    /// The custom sort order used for displaying the product in the user interface.
    /// </summary>
    public int Order { get; set; }
}