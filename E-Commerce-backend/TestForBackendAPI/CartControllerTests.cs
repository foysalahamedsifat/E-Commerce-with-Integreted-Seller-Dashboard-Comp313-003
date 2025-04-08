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
using Xunit;

namespace TestForBackendAPI
{
    public class CartControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly CartController _controller;

        public CartControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _context = new ApplicationDbContext(options);

            // Setup controller with mock HttpContext for User identity
            _controller = new CartController(_context)
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
            UserId = userId,
            ProductId = 1,
            Quantity = 2,
            Product = new Product
            {
                ProductId = 1,
                Name = "Product 1",
                Price = 10.99m,
                Description = "Test Description 1",
                ImageUrl = "http://example.com/product1.jpg"
            }
        },
        new CartItem
        {
            UserId = userId,
            ProductId = 2,
            Quantity = 1,
            Product = new Product
            {
                ProductId = 2,
                Name = "Product 2",
                Price = 20.50m,
                Description = "Test Description 2",
                ImageUrl = "http://example.com/product2.jpg"
            }
        }
    };

            // Add CartItems to the context
            _context.CartItems.AddRange(testData);
            await _context.SaveChangesAsync();

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
            var newItem = new CartItem { ProductId = 3, Quantity = 1, UserId = userId };

            // Act
            var result = await _controller.AddToCart(newItem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added to cart", okResult.Value);

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == 3);
            Assert.NotNull(cartItem);
            Assert.Equal(1, cartItem.Quantity);
        }

        [Fact]
        public async Task AddToCart_UpdatesQuantity_WhenItemAlreadyInCart()
        {
            // Arrange
            var userId = "test-user-id";
            var existingItem = new CartItem { CartItemId = 3, UserId = userId, ProductId = 3, Quantity = 2 };
            _context.CartItems.Add(existingItem);
            await _context.SaveChangesAsync();

            var updateItem = new CartItem { ProductId = 3, Quantity = 3, UserId = userId }; // Ensure UserId is included

            // Act
            var result = await _controller.AddToCart(updateItem);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Item added to cart", okResult.Value);

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == 3);
            Assert.NotNull(cartItem);
            Assert.Equal(5, cartItem.Quantity); // 2 existing + 3 new
        }



        [Fact]
        public async Task RemoveFromCart_ReturnsNoContent_WhenItemExists()
        {
            // Arrange
            var userId = "test-user-id";
            var itemToRemove = new CartItem { CartItemId = 1, UserId = userId, ProductId = 3, Quantity = 1 };
            _context.CartItems.Add(itemToRemove);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            Assert.IsType<NoContentResult>(result);

            var cartItem = await _context.CartItems.FirstOrDefaultAsync(c => c.CartItemId == 1);
            Assert.Null(cartItem);
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsNotFound_WhenItemDoesNotExist()
        {
            // Act
            var result = await _controller.RemoveFromCart(2); // ID that doesn't exist

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task RemoveFromCart_ReturnsNotFound_WhenItemBelongsToDifferentUser()
        {
            // Arrange
            var otherUserItem = new CartItem { CartItemId = 5, UserId = "other-user-id", ProductId = 3, Quantity = 1 };
            _context.CartItems.Add(otherUserItem);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.RemoveFromCart(1);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }
    }
}
