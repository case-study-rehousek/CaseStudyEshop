using AutoMapper;
using Eshop.API.Controllers;
using Eshop.Application.DTO.Request;
using Eshop.Application.DTO.Response;
using Eshop.Application.Interfaces;
using Eshop.Application.Mappings;
using Eshop.Application.Services;
using Eshop.Infrastructure.Mocks;
using Eshop.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;


namespace Eshop.UnitTests
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockService;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockService = new Mock<IProductService>();
            _controller = new ProductsController(_mockService.Object);
        }

        [Fact]
        public async Task GetProduct_WhenProductExists_ReturnsOk()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var expectedProduct = new ProductResponseDto { Id = productId, Name = "Test Product" };

            _mockService.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(expectedProduct);

            // Act
            var result = await _controller.GetProduct(productId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductResponseDto>(okResult.Value);
            Assert.Equal(expectedProduct.Id, returnedProduct.Id);

            _mockService.Verify(x => x.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task GetProduct_WhenProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            
            _mockService
                .Setup(x => x.GetByIdAsync(productId))
                .ReturnsAsync((ProductResponseDto?)null);

            var controller = new ProductsController(_mockService.Object);

            // Act
            var result = await controller.GetProduct(productId);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
            _mockService.Verify(x => x.GetByIdAsync(productId), Times.Once);
        }

        [Fact]
        public async Task CreateProduct_WithCorrectDto_ReturnOk()
        {
            // Arrange
            var requestDto = new CreateProductRequestDto
            {
                Name = "Test",
                ImageUrl = "http://test.jpg",
                Price = 100,
                StockQuantity = 10
            };
            var newId = Guid.NewGuid();

            _mockService.Setup(s => s.CreateAsync(requestDto))
                        .ReturnsAsync(newId);

            // Act
            var result = await _controller.CreateProduct(requestDto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal(newId, createdResult.Value);
        }

        [Fact]
        public async Task CreateProduct_WithOnlyNameAndUrl_ReturnOk()
        {
            // Arrange
            var dto = new CreateProductRequestDto
            {
                Name = "Test",
                ImageUrl = "http://test.jpg"
            };

            var expectedId = Guid.NewGuid();

            _mockService.Setup(s => s.CreateAsync(dto))
                        .ReturnsAsync(expectedId);

            // Act
            var result = await _controller.CreateProduct(dto);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);

            var returnedId = Assert.IsType<Guid>(createdResult.Value);

            Assert.Equal(expectedId, returnedId);

            _mockService.Verify(s => s.CreateAsync(It.Is<CreateProductRequestDto>(d => d.Name == "Test")), Times.Once);
        }

        [Fact]
        public async Task UpdateStock_WhenSuccessful_ReturnsNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var updateDto = new UpdateStockRequestDto { NewQuantity = 50 };

            _mockService.Setup(s => s.UpdateStockAsync(productId, updateDto))
                        .ReturnsAsync(true);

            // Act
            var result = await _controller.UpdateStock(productId, updateDto);

            // Assert
            Assert.IsType<NoContentResult>(result);
            _mockService.Verify(s => s.UpdateStockAsync(productId, updateDto), Times.Once);
        }

        [Fact]
        public async Task UpdateStock_WhenProductDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();
            var dto = new UpdateStockRequestDto { NewQuantity = 100 };

            _mockService.Setup(s => s.UpdateStockAsync(nonExistentId, dto))
                        .ReturnsAsync(false);

            // Act
            var result = await _controller.UpdateStock(nonExistentId, dto);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task GetProducts_WithListOfProducts_ReturnsOk()
        {
            // Arrange
            var mockProducts = new List<ProductResponseDto>
            {
                new() { Id = Guid.NewGuid(), Name = "Product 1" },
                new() { Id = Guid.NewGuid(), Name = "Product 2" }
            };
            _mockService.Setup(s => s.GetAllAsync()).ReturnsAsync(mockProducts);

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProducts = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(okResult.Value);
            Assert.Equal(2, returnedProducts.Count());
        }
    }

    public class ProductControllerTests_DataLayer
    {
        [Fact]
        public async Task GetProduct_WhenProductExists_ReturnsOk()
        {
            // 1. Arrange 
            var mockData = ProductMockData.GetMockProducts();
            var existingProduct = mockData.First();
            var productId = existingProduct.Id;

            var mockRepo = new MockProductRepository();
            
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            var realService = new ProductService(mockRepo, mapper);
            var controller = new ProductsController(realService);

            // 2. Act
            var result = await controller.GetProduct(productId);

            // 3. Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedProduct = Assert.IsType<ProductResponseDto>(okResult.Value);

            Assert.Equal(productId, returnedProduct.Id);
        }
    }
}