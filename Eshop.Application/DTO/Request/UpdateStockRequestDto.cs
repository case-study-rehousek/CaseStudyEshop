using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace Eshop.Application.DTO.Request;

/// <summary>
/// Request object for updating the inventory level of a product.
/// </summary>
public class UpdateStockRequestDto
{
    /// <summary>
    /// The new total stock quantity to be set. Must be 0 or greater.
    /// </summary>
    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
    public int NewQuantity { get; set; }
}