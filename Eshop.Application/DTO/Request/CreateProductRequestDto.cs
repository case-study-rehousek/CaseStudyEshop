using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eshop.Application.DTO.Request;

public class CreateProductRequestDto
{
    public required string Name { get; set; }
    public required string ImageUrl { get; set; }

    public decimal Price { get; set; } = 0;
    public string? Description { get; set; }
    public int StockQuantity { get; set; } = 0;
}
