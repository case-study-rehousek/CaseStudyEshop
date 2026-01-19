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

namespace Eshop.UnitTests.V2
{
    public class ProductsControllerTests
    {
        private readonly ProductsController _controller;        
        private readonly IProductRepository _repo;

        public ProductsControllerTests()
        {
            // 1. Arrange - Setup společné infrastruktury pro všechny testy v této třídě
            _repo = new MockProductRepository();

            // Předpokládáme, že MappingProfile je definován ve vaší Application vrstvě
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            var mapper = config.CreateMapper();

            var service = new ProductService(_repo, mapper);
            _controller = new ProductsController(service);
        }

        /// <summary>
        /// Test základní funkčnosti: Ověřuje, že endpoint vrací OK a data existují.
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
            Assert.Equal(3, products.Count()); // V mocku máme 3 produkty
        }

        /// <summary>
        /// Test velikosti stránky: Ověřuje, že LINQ .Take() správně omezí počet prvků.
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
        /// Test posunu (Skip logic): Ověřuje, že druhá strana neobsahuje stejná data jako první.
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

            Assert.NotEqual(products1.First().Id, products2.First().Id);
        }

        /// <summary>
        /// Test hraničních stavů: Ověřuje, že záporné číslo strany je ošetřeno jako strana 1.
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

            // Oba výsledky musí být identické díky validaci v Controlleru
            Assert.Equal(productsValid.First().Id, productsNegative.First().Id);
        }

        /// <summary>
        /// Test hraničních stavů: Ověřuje, že příliš velký pageSize se zresetuje na výchozí hodnotu (10).
        /// </summary>
        [Fact]
        public async Task GetPagedProducts_BoundaryStates_HugePageSize_ResetsToDefault()
        {
            // Act
            // Uživatel chce 500 prvků, controller by ho měl omezit zpět na 10
            var result = await _controller.GetPagedProducts(pageNumber: 1, pageSize: 500);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            
            var products = Assert.IsAssignableFrom<IEnumerable<ProductResponseDto>>(okResult.Value);
            Assert.Equal(3, products.Count());
        }
    }
}
