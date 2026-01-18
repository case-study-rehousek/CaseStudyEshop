using System;
using System.Collections.Generic;
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
    public required string Name { get; set; }

    /// <summary>
    /// The absolute URL to the product's image. This field is required.
    /// </summary>
    public required string ImageUrl { get; set; }

    /// <summary>
    /// The unit price of the product. Defaults to 0 if not specified.
    /// </summary>
    public decimal Price { get; set; } = 0;

    /// <summary>
    /// An optional detailed description of the product.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// The initial amount of items in stock. Defaults to 0 if not specified.
    /// </summary>
    public int StockQuantity { get; set; } = 0;
}
