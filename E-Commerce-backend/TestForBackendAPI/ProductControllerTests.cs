using E_Commerce_Backend.Models;
using JWTAuthentication.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xunit;
using JWTAuthentication.Authentication;

namespace TestForBackendAPI
{
    public class ProductControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly ProductController _controller;

        public ProductControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // use unique DB for each test
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new ProductController(_context);
        }

        [Fact]
        public async Task GetProducts_ReturnsOkResult_WithProducts()
        {
            // Arrange
            _context.Products.AddRange(
                new Product { Name = "Product1", Price = 10, Description = "Description1", ImageUrl = "/images/product1.jpg" },
                new Product { Name = "Product2", Price = 20, Description = "Description2", ImageUrl = "/images/product2.jpg" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var products = Assert.IsType<List<Product>>(okResult.Value);
            Assert.Equal(2, products.Count);
        }

        [Fact]
        public async Task GetProduct_ReturnsOkResult_WithProduct()
        {
            // Arrange
            var product = new Product { Name = "Product1", Price = 10, Description = "Description1", ImageUrl = "/images/product1.jpg" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.GetProduct(product.ProductId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedProduct = Assert.IsType<Product>(okResult.Value);
            Assert.Equal(product.Name, returnedProduct.Name);
        }

        [Fact]
        public async Task GetProduct_ReturnsNotFound_WhenProductDoesNotExist()
        {
            // Act
            var result = await _controller.GetProduct(999);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task AddProduct_ReturnsCreatedAtActionResult_WithProduct()
        {
            // Arrange
            var product = new Product
            {
                Name = "Test Product",
                Price = 99,
                Description = "Test Description", // Description added
                ImageUrl = "/images/test-product.jpg" // ImageUrl added
            };

            var imageContent = "FakeImageContent";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(imageContent);
            writer.Flush();
            ms.Position = 0;

            var formFile = new FormFile(ms, 0, ms.Length, "image", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            // Act
            var result = await _controller.AddProduct(product, formFile);

            // Assert
            var createdResult = Assert.IsType<CreatedAtActionResult>(result);
            var returnProduct = Assert.IsType<Product>(createdResult.Value);
            Assert.Equal("Test Product", returnProduct.Name);
            Assert.Contains("/images/", returnProduct.ImageUrl);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsBadRequest_WhenIdMismatch()
        {
            var product = new Product { ProductId = 1, Name = "Mismatch", Price = 50 };
            var fakeImage = new Mock<IFormFile>();
            var result = await _controller.UpdateProduct(2, product, fakeImage.Object);
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateProduct_ReturnsNoContent_WhenUpdated()
        {
            // Arrange
            var product = new Product { Name = "Product1", Price = 10, Description = "Description1", ImageUrl = "/images/product1.jpg" };
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            // Detach the product to prevent it from being tracked twice
            _context.Entry(product).State = EntityState.Detached;

            var updated = new Product
            {
                ProductId = product.ProductId,
                Name = "Updated",
                Price = 20,
                Description = "Updated Description",
                ImageUrl = "/images/updated-product.jpg"
            };

            var fakeImage = new Mock<IFormFile>();

            // Act
            var result = await _controller.UpdateProduct(updated.ProductId, updated, fakeImage.Object);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }


        [Fact]
        public async Task DeleteProduct_ReturnsNoContent_WhenDeleted()
        {
            var product = new Product
            {
                Name = "ToDelete",
                Price = 30,
                Description = "To be deleted",
                ImageUrl = "/images/sample.jpg" // ImageUrl added
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            var result = await _controller.DeleteProduct(product.ProductId);

            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteProduct_ReturnsNotFound_WhenNotExists()
        {
            var result = await _controller.DeleteProduct(999);
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
