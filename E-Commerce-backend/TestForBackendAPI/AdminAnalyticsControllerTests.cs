using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce_Backend.Models;
using JWTAuthentication.Authentication;
using JWTAuthentication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace TestForBackendAPI
{
    public class AdminAnalyticsControllerTests
    {
        private readonly ApplicationDbContext _context;
        private readonly AdminAnalyticsController _controller;

        public AdminAnalyticsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                            .UseInMemoryDatabase(databaseName: "TestDatabase")
                            .Options;

            _context = new ApplicationDbContext(options);
            _controller = new AdminAnalyticsController(_context);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var users = new List<ApplicationUser>
            {
                new ApplicationUser
                {
                    FirstName = "John",
                    LastName = "Doe",
                    Address = "123 Main St",
                    PostalCode = "12345",
                    UserName = "john.doe@example.com",
                    Email = "john.doe@example.com"
                },
                new ApplicationUser
                {
                    FirstName = "Jane",
                    LastName = "Smith",
                    Address = "456 Elm St",
                    PostalCode = "67890",
                    UserName = "jane.smith@example.com",
                    Email = "jane.smith@example.com"
                }
            };

            var products = new List<Product>
            {
                new Product {
                    Name = "Product1",
                    Description = "Description1",
                    Price = 10,
                    Stock = 100,
                    ImageUrl = "https://example.com/product1.jpg" // Added required ImageUrl
                },
                new Product {
                    Name = "Product2",
                    Description = "Description2",
                    Price = 20,
                    Stock = 200,
                    ImageUrl = "https://example.com/product2.jpg" // Added required ImageUrl
                }
            };

            var orders = new List<Order>
            {
                new Order { TotalAmount = 100, Status = "Pending", OrderDate = DateTime.Now, User = users[0] },
                new Order { TotalAmount = 200, Status = "Completed", OrderDate = DateTime.Now, User = users[1] }
            };

            var orderDetails = new List<OrderDetail>
            {
                new OrderDetail { Order = orders[0], Product = products[0], Quantity = 1, Price = 10 },
                new OrderDetail { Order = orders[1], Product = products[1], Quantity = 2, Price = 20 }
            };

            _context.Users.AddRange(users);
            _context.Products.AddRange(products);
            _context.Orders.AddRange(orders);
            _context.OrderDetails.AddRange(orderDetails);
            _context.SaveChanges();
        }

        [Fact]
        public async Task GetDashboardSummary_ReturnsOkResult_WithSummaryData()
        {
            // Act
            var result = await _controller.GetDashboardSummary();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var summary = okResult.Value as dynamic;
            Assert.Equal(2, summary.TotalUsers);
            Assert.Equal(2, summary.TotalOrders);
            Assert.Equal(300m, summary.TotalRevenue);
            Assert.Equal(1, summary.PendingOrders);
            Assert.Equal(1, summary.CompletedOrders);
        }

        [Fact]
        public async Task GetBestSellingProducts_ReturnsOkResult_WithBestSellingProducts()
        {
            // Act
            var result = await _controller.GetBestSellingProducts();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var bestSellingProducts = okResult.Value as List<dynamic>;
            Assert.Equal(2, bestSellingProducts.Count);
            Assert.Equal("Product2", bestSellingProducts[0].ProductName);
            Assert.Equal(2, bestSellingProducts[0].TotalQuantitySold);
            Assert.Equal(40m, bestSellingProducts[0].TotalRevenueGenerated);
        }

        [Fact]
        public async Task GetSalesReport_ReturnsOkResult_WithSalesData()
        {
            // Arrange
            var startDate = DateTime.Now.AddDays(-1);
            var endDate = DateTime.Now.AddDays(1);

            // Act
            var result = await _controller.GetSalesReport(startDate, endDate);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var salesData = okResult.Value as List<dynamic>;
            Assert.Single(salesData);
            Assert.Equal(2, salesData[0].TotalOrders);
            Assert.Equal(300m, salesData[0].TotalRevenue);
        }

        [Fact]
        public async Task GetUserStatistics_ReturnsOkResult_WithUserStatistics()
        {
            // Act
            var result = await _controller.GetUserStatistics();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userStats = okResult.Value as List<dynamic>;
            Assert.Equal(2, userStats.Count);
            Assert.Equal("John", userStats[0].Date); // First name of the first user in alphabetical order
            Assert.Equal(1, userStats[0].NewUsers);
        }

        [Fact]
        public async Task GetOrderStatusDistribution_ReturnsOkResult_WithOrderStatusData()
        {
            // Act
            var result = await _controller.GetOrderStatusDistribution();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orderStatusData = okResult.Value as List<dynamic>;
            Assert.Equal(2, orderStatusData.Count);
            Assert.Contains(orderStatusData, x => x.Status == "Pending" && x.Count == 1);
            Assert.Contains(orderStatusData, x => x.Status == "Completed" && x.Count == 1);
        }
    }
}