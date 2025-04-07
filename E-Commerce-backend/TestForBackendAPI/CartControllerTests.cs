using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using E_Commerce_Backend.Models;
using JWTAuthentication.Authentication;
using JWTAuthentication.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace TestForBackendAPI
{
    public class CartControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            _mockContext = new Mock<ApplicationDbContext>(new DbContextOptions<ApplicationDbContext>());

            // Setup controller with mock HttpContext for User identity
            _controller = new CartController(_mockContext.Object)
            {
                ControllerContext = new ControllerContext()
                {
                    HttpContext = new DefaultHttpContext()
                    {
                        User = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                        {
                            new Claim(ClaimTypes.NameIdentifier, "test-user-id")
                        }))
                    }
                }
            };
        }

        [Fact]
        public async Task GetCartItems_ReturnsOkResult_WithUserCartItems()
        {
            // Arrange
            var userId = "test-user-id";
            var testData = new List<CartItem>
    {
        new CartItem
        {
            CartItemId = 1,
            UserId = userId,
            ProductId = 1,
            Quantity = 2,
            Product = new Product { ProductId = 1, Name = "Product 1", Price = 10.99m }
        },
        new CartItem
        {
            CartItemId = 2,
            UserId = userId,
            ProductId = 2,
            Quantity = 1,
            Product = new Product { ProductId = 2, Name = "Product 2", Price = 20.50m }
        }
    }.AsQueryable();

            var mockSet = new Mock<DbSet<CartItem>>();
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());
            mockSet.Setup(m => m.Include(It.IsAny<string>())).Returns(mockSet.Object);

            _mockContext.Setup(c => c.CartItems).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetCartItems();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var cartItems = Assert.IsType<List<CartItem>>(okResult.Value);
            Assert.Equal(2, cartItems.Count);
            Assert.All(cartItems, item => Assert.Equal(userId, item.UserId));
        }


        [Fact]
        public async Task AddToCart_AddsNewItem_WhenItemNotInCart()
        {
            // Arrange
            var userId = "test-user-id";
            var newItem = new CartItem { ProductId = 3, Quantity = 1, UserId = userId }; // Ensure UserId is set

            // Create a mock DbSet that supports Add() and IQueryable functionality
            var mockSet = new Mock<DbSet<CartItem>>();

            // Use a list to simulate the CartItems table and set up IQueryable
            var existingCart = new List<CartItem>().AsQueryable();

            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Provider).Returns(existingCart.Provider);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Expression).Returns(existingCart.Expression);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.ElementType).Returns(existingCart.ElementType);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.GetEnumerator()).Returns(existingCart.GetEnumerator());

            // Set up SaveChangesAsync to simulate saving to the database
            mockSet.Setup(m => m.Add(It.IsAny<CartItem>())).Callback<CartItem>(item => existingCart.ToList().Add(item));

            // Set up the mock context
            _mockContext.Setup(c => c.Set<CartItem>()).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.AddToCart(newItem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added to cart", okResult.Value);

            // Verify that Add() was called with the correct CartItem
            mockSet.Verify(m => m.Add(It.Is<CartItem>(x => x.UserId == userId && x.ProductId == 3)), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }




        [Fact]
        public async Task AddToCart_UpdatesQuantity_WhenItemAlreadyInCart()
        {
            // Arrange
            var userId = "test-user-id";
            var existingItem = new CartItem { CartItemId = 1, UserId = userId, ProductId = 3, Quantity = 2 };
            var updateItem = new CartItem { ProductId = 3, Quantity = 3 };

            var testData = new List<CartItem> { existingItem }.AsQueryable();

            var mockSet = new Mock<DbSet<CartItem>>();
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            // Set up SaveChangesAsync to simulate saving to the database
            mockSet.Setup(m => m.Update(It.IsAny<CartItem>())).Callback<CartItem>(item =>
            {
                var existing = testData.FirstOrDefault(x => x.ProductId == item.ProductId && x.UserId == item.UserId);
                if (existing != null)
                {
                    existing.Quantity += item.Quantity;
                }
            });

            // Set up the mock context
            _mockContext.Setup(c => c.Set<CartItem>()).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.AddToCart(updateItem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added to cart", okResult.Value);
            Assert.Equal(5, existingItem.Quantity); // 2 existing + 3 new
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }


        [Fact]
        public async Task RemoveFromCart_ReturnsNoContent_WhenItemExists()
        {
            // Arrange
            var userId = "test-user-id";
            var itemToRemove = new CartItem { CartItemId = 1, UserId = userId, ProductId = 3, Quantity = 1 };

            var mockSet = new Mock<DbSet<CartItem>>();
            var testData = new List<CartItem> { itemToRemove }.AsQueryable();

            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            _mockContext.Setup(c => c.CartItems).Returns(mockSet.Object);
            _mockContext.Setup(c => c.SaveChangesAsync(default)).ReturnsAsync(1);

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
            mockSet.Verify(m => m.Remove(itemToRemove), Times.Once);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Once);
        }


        [Fact]
        public async Task RemoveFromCart_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Arrange
            var userId = "test-user-id";
            var existingItem = new CartItem { CartItemId = 1, UserId = userId, ProductId = 3, Quantity = 1 };

            var mockSet = new Mock<DbSet<CartItem>>();
            var testData = new List<CartItem> { existingItem }.AsQueryable();

            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            _mockContext.Setup(c => c.CartItems).Returns(mockSet.Object);

            // Act
            var result = await _controller.RemoveFromCart(2); // ID that doesn't exist

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockSet.Verify(m => m.Remove(It.IsAny<CartItem>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }


        [Fact]
        public async Task RemoveFromCart_ReturnsNotFound_WhenItemBelongsToDifferentUser()
        {
            // Arrange
            var userId = "test-user-id";
            var otherUserItem = new CartItem { CartItemId = 1, UserId = "other-user-id", ProductId = 3, Quantity = 1 };

            var mockSet = new Mock<DbSet<CartItem>>();
            var testData = new List<CartItem> { otherUserItem }.AsQueryable();

            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Provider).Returns(testData.Provider);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.Expression).Returns(testData.Expression);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.ElementType).Returns(testData.ElementType);
            mockSet.As<IQueryable<CartItem>>().Setup(m => m.GetEnumerator()).Returns(testData.GetEnumerator());

            _mockContext.Setup(c => c.CartItems).Returns(mockSet.Object);

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            mockSet.Verify(m => m.Remove(It.IsAny<CartItem>()), Times.Never);
            _mockContext.Verify(m => m.SaveChangesAsync(default), Times.Never);
        }

    }
}