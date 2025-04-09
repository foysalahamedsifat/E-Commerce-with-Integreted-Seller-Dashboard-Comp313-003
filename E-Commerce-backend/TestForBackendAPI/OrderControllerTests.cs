using E_Commerce_Backend.Dto;
using E_Commerce_Backend.Models;
using JWTAuthentication.Authentication;
using JWTAuthentication.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xunit;

namespace TestForBackendAPI
{
    public class OrderControllerTests
    {
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase") // Unique in-memory database for each test
                .Options;

            var context = new ApplicationDbContext(options);

            // Initialize the controller with the in-memory database context
            _controller = new OrderController(context);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "test-user-id")
            }, "mock"));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };
        }

        [Fact]
        public async Task GetOrders_ReturnsOkResult_WithOrders()
        {
            // Arrange
            var userId = "test-user-id";
            var orders = new List<Order>
            {
                new Order { OrderId = 4, UserId = userId, OrderDetails = new List<OrderDetail>() }
            };

            // Seed data into the in-memory database
            using (var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options))
            {
                context.Orders.AddRange(orders);
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnOrders = Assert.IsType<List<Order>>(okResult.Value);
            Assert.Single(returnOrders);
        }

        [Fact]
        public async Task GetOrdersByUserId_ReturnsBadRequest_WhenUserIdIsNull()
        {
            // Arrange
            _controller.ControllerContext.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity());

            // Act
            var result = await _controller.GetOrdersByUserId();

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("User ID is required", badRequestResult.Value);
        }

        [Fact]
        public async Task GetOrdersByUserId_ReturnsNotFound_WhenNoOrdersFound()
        {
            // Arrange
            var userId = "test-user-id";

            // Seed data into the in-memory database with no orders
            using (var context = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options))
            {
                // No orders added
                await context.SaveChangesAsync();
            }

            // Act
            var result = await _controller.GetOrdersByUserId();

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal($"No orders found for user ID: {userId}", notFoundResult.Value);
        }


        [Fact]
        public async Task PlaceOrder_ReturnsBadRequest_WhenCartIsEmpty()
        {
            // Arrange
            var cartItems = new List<CartItemDto>();

            // Act
            var result = await _controller.PlaceOrder(cartItems);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Cart is empty", badRequestResult.Value);
        }

        [Fact]
        public async Task PlaceOrder_ReturnsOkResult_WhenOrderIsPlaced()
        {
            // Arrange
            var cartItems = new List<CartItemDto>
            {
                new CartItemDto { ProductId = 1, Quantity = 2, Price = 10 }
            };

            // Act
            var result = await _controller.PlaceOrder(cartItems);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Order placed successfully", okResult.Value);
        }
    }
}
