using JWTAuthentication.Controllers;
using JWTAuthentication.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;

namespace TestForBackendAPI
{
    public class AuthenticateControllerTests
    {
        private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
        private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthenticateController _controller;

        public AuthenticateControllerTests()
        {
            _userManagerMock = new Mock<UserManager<ApplicationUser>>(
                new Mock<IUserStore<ApplicationUser>>().Object, null, null, null, null, null, null, null, null);
            _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
                new Mock<IRoleStore<IdentityRole>>().Object, null, null, null, null);
            _configurationMock = new Mock<IConfiguration>();

            _controller = new AuthenticateController(_userManagerMock.Object, _roleManagerMock.Object, _configurationMock.Object);
        }

        [Fact]
        public async Task Login_ReturnsBadRequest_WhenModelIsNull()
        {
            // Act
            var result = await _controller.Login(null);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var value = Assert.IsType<Dictionary<string, string>>(badRequestResult.Value);
            Assert.Equal("Username and Password are required.", value["message"]);
        }

        [Fact]
        public async Task Login_ReturnsUnauthorized_WhenInvalidCredentials()
        {
            // Arrange
            var model = new LoginModel { Username = "test", Password = "@Password123" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);

            // Act
            var result = await _controller.Login(model);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            var value = Assert.IsType<Dictionary<string, string>>(unauthorizedResult.Value);
            Assert.Equal("Invalid credentials.", value["message"]);
        }

        [Fact]
        public async Task Register_ReturnsConflict_WhenUserExists()
        {
            // Arrange
            var model = new RegisterModel { Username = "test", Email = "test@test.com", Password = "password" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            // Act
            var result = await _controller.Register(model);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, conflictResult.StatusCode);
            var value = Assert.IsType<Response>(conflictResult.Value);
            Assert.Equal("User already exists!", value.Message);
        }

        [Fact]
        public async Task Register_ReturnsOk_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var model = new RegisterModel { Username = "test", Email = "test@test.com", Password = "password" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<Response>(okResult.Value);
            Assert.Equal("User created successfully!", value.Message);
        }

        [Fact]
        public async Task RegisterAdmin_ReturnsConflict_WhenUserExists()
        {
            // Arrange
            var model = new RegisterModel { Username = "test", Email = "test@test.com", Password = "password" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(new ApplicationUser());

            // Act
            var result = await _controller.RegisterAdmin(model);

            // Assert
            var conflictResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, conflictResult.StatusCode);
            var value = Assert.IsType<Response>(conflictResult.Value);
            Assert.Equal("User already exists!", value.Message);
        }

        [Fact]
        public async Task RegisterAdmin_ReturnsOk_WhenUserCreatedSuccessfully()
        {
            // Arrange
            var model = new RegisterModel { Username = "test", Email = "test@test.com", Password = "password" };
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync((ApplicationUser)null);
            _userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _roleManagerMock.Setup(x => x.RoleExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

            // Act
            var result = await _controller.RegisterAdmin(model);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var value = Assert.IsType<Response>(okResult.Value);
            Assert.Equal("User created successfully!", value.Message);
        }
    }

}