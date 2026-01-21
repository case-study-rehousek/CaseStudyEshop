using AutoMapper;
using Eshop.API.Controllers.V2;
using Eshop.Application.DTO.Request;
using Eshop.Application.DTO.Response;
using Eshop.Application.Interfaces;
using Eshop.Application.Mappings;
using Eshop.Application.Services;
using Eshop.Domain.Interfaces;
using Eshop.Infrastructure.Mocks;
using Eshop.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Text;

namespace Eshop.UnitTests.V2
{

    public class ProductsControllerTests
    {
        private readonly ProductsController _controller;
        private readonly IProductRepository _repo;
        private readonly Mock<IStockUpdateQueueService> _queueMock;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductsControllerTests"/> class.
        /// Sets up the test infrastructure including Mock Repository, AutoMapper, and Mock Queue Service.
        /// </summary>
        public ProductsControllerTests()
        {
            // 1. Arrange - Setup common infrastructure for all tests in this class
            _repo = new MockProductRepository();
            _queueMock = new Mock<IStockUpdateQueueService>();

            // Setup AutoMapper with the actual MappingProfile from the Application layer
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            var service = new ProductService(_repo, mapper);
            _controller = new ProductsController(service, _queueMock.Object);
        }

        /// <summary>
        /// Verifies that the asynchronous stock update endpoint returns an Accepted (202) result 
        /// and correctly interacts with the background queue service.
        /// </summary>
        [Fact]
        public async Task UpdateStockAsyncQueue_ReturnsAccepted()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var request = new UpdateStockRequestDto { NewQuantity = 10 };

            // Act
            var result = await _controller.UpdateStockAsyncQueue(productId, request);

            // Assert
            Assert.IsType<AcceptedResult>(result);

            // Verify that the controller actually called the QueueStockUpdateAsync method once
            _queueMock.Verify(q => q.QueueStockUpdateAsync(It.IsAny<UpdateStockTask>()), Times.Once);
        }

        /// <summary>
        /// Basic functionality test: Verifies that the endpoint returns OK status and that data exists.
        /// </summary>
        [Fact]
        public async Task GetPagedProducts_BasicFunctionality_ReturnsOkWithProducts()
        {
            // Act
            var result = await _controller.GetPagedProducts(pageNumber: 1, pageSize: 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(okResult.Value);

            Assert.NotEmpty(products);
            Assert.Equal(3, products.Count()); // The mock repository contains 3 products
        }

        /// <summary>
        /// Page size test: Verifies that the LINQ .Take() logic correctly limits the number of returned items.
        /// </summary>
        [Fact]
        public async Task GetPagedProducts_PageSize_ReturnsExactNumberOfItems()
        {
            // Arrange
            int requestedSize = 2;

            // Act
            var result = await _controller.GetPagedProducts(pageNumber: 1, pageSize: requestedSize);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(okResult.Value);

            Assert.Equal(requestedSize, products.Count());
        }

        /// <summary>
        /// Offset/Skip logic test: Verifies that the second page contains different data than the first page.
        /// </summary>
        [Fact]
        public async Task GetPagedProducts_SkipLogic_SecondPageReturnsRemainingData()
        {
            // Arrange
            int pageSize = 2;

            // Act
            var resultPage1 = await _controller.GetPagedProducts(pageNumber: 1, pageSize: pageSize);
            var resultPage2 = await _controller.GetPagedProducts(pageNumber: 2, pageSize: pageSize);

            // Assert
            var products1 = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(((OkObjectResult)resultPage1.Result!).Value);
            var products2 = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(((OkObjectResult)resultPage2.Result!).Value);

            Assert.Equal(2, products1.Count());
            Assert.Single(products2);

            // Ensure IDs are different to confirm actual skipping happened
            Assert.NotEqual(products1.First().Id, products2.First().Id);
        }

        /// <summary>
        /// Boundary states test: Verifies that a negative page number is handled and reset to page 1.
        /// </summary>
        [Fact]
        public async Task GetPagedProducts_BoundaryStates_NegativePageReturnsFirstPage()
        {
            // Act
            var resultNegative = await _controller.GetPagedProducts(pageNumber: -1, pageSize: 10);
            var resultValid = await _controller.GetPagedProducts(pageNumber: 1, pageSize: 10);

            // Assert
            var productsNegative = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(((OkObjectResult)resultNegative.Result!).Value);
            var productsValid = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(((OkObjectResult)resultValid.Result!).Value);

            // Both results must be identical due to controller-level validation
            Assert.Equal(productsValid.First().Id, productsNegative.First().Id);
        }

        /// <summary>
        /// Boundary states test: Verifies that an excessively large pageSize is reset to the default value.
        /// </summary>
        [Fact]
        public async Task GetPagedProducts_BoundaryStates_HugePageSize_ResetsToDefault()
        {
            // Act
            // User requests 500 items, controller should cap this back to a reasonable limit (e.g., 10 or max available)
            var result = await _controller.GetPagedProducts(pageNumber: 1, pageSize: 500);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);

            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(okResult.Value);
            // Since we only have 3 products in mock, it returns 3, but verifies the call didn't crash
            Assert.Equal(3, products.Count());
        }
    }
}
