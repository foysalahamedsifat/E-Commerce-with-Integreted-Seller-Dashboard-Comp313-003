using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using E_Commerce_Backend.Models;
using JWTAuthentication.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using JWTAuthentication.Authentication;

namespace TestForBackendAPI
{
    public class AdminAnalyticsControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly AdminAnalyticsController _controller;

        public AdminAnalyticsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>().Options;
            _mockContext = new Mock<ApplicationDbContext>(options);
            _controller = new AdminAnalyticsController(_mockContext.Object);
        }

        [Fact]
        public async Task GetDashboardSummary_ReturnsOkResult_WithSummaryData()
        {
            // Arrange
            var mockUsers = new Mock<DbSet<ApplicationUser>>();
            var mockOrders = new Mock<DbSet<Order>>();

            _mockContext.Setup(c => c.Users).Returns(mockUsers.Object);
            _mockContext.Setup(c => c.Orders).Returns(mockOrders.Object);

            _mockContext.Setup(c => c.Users.CountAsync(default)).ReturnsAsync(10);
            _mockContext.Setup(c => c.Orders.CountAsync(default)).ReturnsAsync(20);
            _mockContext.Setup(c => c.Orders.SumAsync(o => o.TotalAmount, default)).ReturnsAsync(1000m);
            _mockContext.Setup(c => c.Orders.CountAsync(o => o.Status == "Pending", default)).ReturnsAsync(5);
            _mockContext.Setup(c => c.Orders.CountAsync(o => o.Status == "Completed", default)).ReturnsAsync(15);

            // Act
            var result = await _controller.GetDashboardSummary();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var summary = okResult.Value as dynamic;
            Assert.Equal(10, summary.TotalUsers);
            Assert.Equal(20, summary.TotalOrders);
            Assert.Equal(1000m, summary.TotalRevenue);
            Assert.Equal(5, summary.PendingOrders);
            Assert.Equal(15, summary.CompletedOrders);
        }
        [Fact]
        public async Task GetBestSellingProducts_ReturnsOkResult_WithBestSellingProducts()
        {
            // Arrange
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new ApplicationDbContext(options))
            {
                var product1 = new Product { ProductId = 1, Name = "Product1", Description = "Description1", ImageUrl = "ImageUrl1" };
                var product2 = new Product { ProductId = 2, Name = "Product2", Description = "Description2", ImageUrl = "ImageUrl2" };

                context.Products.AddRange(product1, product2);
                context.OrderDetails.AddRange(
                    new OrderDetail { ProductId = 1, Product = product1, Quantity = 10, Price = 100 },
                    new OrderDetail { ProductId = 2, Product = product2, Quantity = 5, Price = 200 },
                    new OrderDetail { ProductId = 1, Product = product1, Quantity = 5, Price = 100 }
                );
                context.SaveChanges();
            }

            using (var context = new ApplicationDbContext(options))
            {
                var controller = new AdminAnalyticsController(context);

                // Act
                var result = await controller.GetBestSellingProducts();

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result);
                var products = Assert.IsType<List<object>>(okResult.Value);
                Assert.Equal(2, products.Count);

                // The first product should be Product1 with total quantity of 15
                dynamic product1 = products[0];
                Assert.Equal("Product1", product1.ProductName);
                Assert.Equal(15, product1.TotalQuantitySold);
                Assert.Equal(1500m, product1.TotalRevenueGenerated);

                // The second product should be Product2 with total quantity of 5
                dynamic product2 = products[1];
                Assert.Equal("Product2", product2.ProductName);
                Assert.Equal(5, product2.TotalQuantitySold);
                Assert.Equal(1000m, product2.TotalRevenueGenerated);
            }
        }



        [Fact]
        public async Task GetSalesReport_ReturnsOkResult_WithSalesData()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { OrderDate = new DateTime(2023, 1, 1), TotalAmount = 100 },
                new Order { OrderDate = new DateTime(2023, 1, 2), TotalAmount = 200 },
                new Order { OrderDate = new DateTime(2023, 1, 1), TotalAmount = 150 }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Order>>();
            mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            _mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetSalesReport(new DateTime(2023, 1, 1), new DateTime(2023, 1, 2));

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var salesData = Assert.IsType<List<dynamic>>(okResult.Value);

            // Should have two dates: 2023-01-01 and 2023-01-02
            Assert.Equal(2, salesData.Count);

            // Check the data for 2023-01-01
            Assert.Equal(new DateTime(2023, 1, 1), salesData[0].Date);
            Assert.Equal(2, salesData[0].TotalOrders);
            Assert.Equal(250m, salesData[0].TotalRevenue);

            // Check the data for 2023-01-02
            Assert.Equal(new DateTime(2023, 1, 2), salesData[1].Date);
            Assert.Equal(1, salesData[1].TotalOrders);
            Assert.Equal(200m, salesData[1].TotalRevenue);
        }

        [Fact]
        public async Task GetUserStatistics_ReturnsOkResult_WithUserStatistics()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = "1", FirstName = "John" },
                new ApplicationUser { Id = "2", FirstName = "Jane" },
                new ApplicationUser { Id = "3", FirstName = "John" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<ApplicationUser>>();
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Provider).Returns(users.Provider);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.Expression).Returns(users.Expression);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.ElementType).Returns(users.ElementType);
            mockSet.As<IQueryable<ApplicationUser>>().Setup(m => m.GetEnumerator()).Returns(users.GetEnumerator());

            _mockContext.Setup(c => c.Users).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetUserStatistics();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var userStats = Assert.IsType<List<dynamic>>(okResult.Value);

            // Should group by FirstName
            Assert.Equal(2, userStats.Count);

            // John should have 2 users
            Assert.Equal("John", userStats[0].FirstName);
            Assert.Equal(2, userStats[0].Count);

            // Jane should have 1 user
            Assert.Equal("Jane", userStats[1].FirstName);
            Assert.Equal(1, userStats[1].Count);
        }

        [Fact]
        public async Task GetOrderStatusDistribution_ReturnsOkResult_WithOrderStatusData()
        {
            // Arrange
            var orders = new List<Order>
            {
                new Order { Status = "Pending" },
                new Order { Status = "Completed" },
                new Order { Status = "Pending" },
                new Order { Status = "Shipped" },
                new Order { Status = "Completed" }
            }.AsQueryable();

            var mockSet = new Mock<DbSet<Order>>();
            mockSet.As<IQueryable<Order>>().Setup(m => m.Provider).Returns(orders.Provider);
            mockSet.As<IQueryable<Order>>().Setup(m => m.Expression).Returns(orders.Expression);
            mockSet.As<IQueryable<Order>>().Setup(m => m.ElementType).Returns(orders.ElementType);
            mockSet.As<IQueryable<Order>>().Setup(m => m.GetEnumerator()).Returns(orders.GetEnumerator());

            _mockContext.Setup(c => c.Orders).Returns(mockSet.Object);

            // Act
            var result = await _controller.GetOrderStatusDistribution();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var orderStatusData = Assert.IsType<List<dynamic>>(okResult.Value);

            // Should have 3 statuses
            Assert.Equal(3, orderStatusData.Count);

            // Check counts for each status
            var pending = orderStatusData.FirstOrDefault(x => x.Status == "Pending");
            var completed = orderStatusData.FirstOrDefault(x => x.Status == "Completed");
            var shipped = orderStatusData.FirstOrDefault(x => x.Status == "Shipped");

            Assert.NotNull(pending);
            Assert.Equal(2, pending.Count);

            Assert.NotNull(completed);
            Assert.Equal(2, completed.Count);

            Assert.NotNull(shipped);
            Assert.Equal(1, shipped.Count);
        }
    }
}