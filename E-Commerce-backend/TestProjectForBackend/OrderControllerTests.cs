using Moq;

namespace TestProjectForBackend
{
    public class OrderControllerTests
    {
        private readonly Mock<ApplicationDbContext> _mockContext;
        private readonly OrderController _controller;

        public OrderControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _mockContext = new Mock<ApplicationDbContext>(options);
            _controller = new OrderController(_mockContext.Object);

            // Mock user identity
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
            var orders = new List<Order>
        {
            new Order { UserId = "test-user-id", OrderDetails = new List<OrderDetail>() }
        };
            _mockContext.Setup(c => c.Orders).ReturnsDbSet(orders);

            // Act
            var result = await _controller.GetOrders();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnOrders = Assert.IsType<List<Order>>(okResult.Value);
            Assert.Single(returnOrders);
        }

        [Fact]
        public async Task PlaceOrder_ReturnsOkResult_WhenOrderIsPlaced()
        {
            // Arrange
            var cartItems = new List<CartItemDto>
        {
            new CartItemDto { ProductId = 1, Quantity = 2, Price = 10.0m }
        };

            // Act
            var result = await _controller.PlaceOrder(cartItems);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal("Order placed successfully", okResult.Value);
        }
    }

}