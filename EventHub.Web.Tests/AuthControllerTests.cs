using EventHub.Web.Controllers.Api;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace EventHub.Web.Tests
{
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<IdentityUser>> _mockUserManager;
        private readonly Mock<SignInManager<IdentityUser>> _mockSignInManager;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly AuthController _controller;

        public AuthControllerTests()
        {
            var userStore = new Mock<IUserStore<IdentityUser>>();
            _mockUserManager = new Mock<UserManager<IdentityUser>>(userStore.Object, null, null, null, null, null, null, null, null);
            _mockSignInManager = new Mock<SignInManager<IdentityUser>>(_mockUserManager.Object, null, null, null, null, null, null);
            _mockConfiguration = new Mock<IConfiguration>();
            _mockConfiguration.Setup(c => c["Jwt:Issuer"]).Returns("EventHub");
            _mockConfiguration.Setup(c => c["Jwt:Audience"]).Returns("EventHubApi");
            _mockConfiguration.Setup(c => c["Jwt:Key"]).Returns("YourSuperSecretKey1234567890!@#$%^&*()");
            _controller = new AuthController(_mockUserManager.Object, _mockSignInManager.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task Login_ValidCredentials_ReturnsToken()
        {
            var model = new LoginModel { Email = "test@eventhub.com", Password = "Password123!" };
            var user = new IdentityUser { Email = model.Email, UserName = "testuser" };
            _mockUserManager.Setup(u => u.FindByEmailAsync(model.Email)).ReturnsAsync(user);
            _mockSignInManager.Setup(s => s.CheckPasswordSignInAsync(user, model.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);
            _mockUserManager.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new[] { "User" });

            var result = await _controller.Login(model);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var response = Assert.IsAssignableFrom<object>(okResult.Value);
            Assert.NotNull(response.GetType().GetProperty("Token")?.GetValue(response));
        }

        [Fact]
        public async Task Login_InvalidCredentials_ReturnsUnauthorized()
        {
            var model = new LoginModel { Email = "test@eventhub.com", Password = "WrongPassword" };
            _mockUserManager.Setup(u => u.FindByEmailAsync(model.Email)).ReturnsAsync((IdentityUser)null);

            var result = await _controller.Login(model);

            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }
}