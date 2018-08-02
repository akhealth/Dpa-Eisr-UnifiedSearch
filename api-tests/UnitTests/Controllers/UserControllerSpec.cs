using Xunit;
using SearchApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace SearchApi.Tests.UnitTests.Controllers
{
    public class UserControllerSpec
    {
        private ILogger<UserController> _logger = new Mock<ILogger<UserController>>().Object;

        [Fact]
        public void GetUserInfo_should_return_OkObjectResult()
        {
            // Arrange
            var controller = new UserController(new HostingEnvironment(), _logger);

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = controller.GetUserInfo() as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void GetUserInfo_should_return_Developer_in_development_environment()
        {
            // Arrange
            var controller = new UserController(new HostingEnvironment()
            {
                EnvironmentName = "Development"
            }, _logger);

            controller.ControllerContext = new ControllerContext();
            controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = controller.GetUserInfo() as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void RefreshSession_returns_OkObjectResult()
        {
            // Arrange
            var controller = new UserController(new HostingEnvironment(), _logger);

            // Act
            var result = controller.RefreshSession() as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public void EndSession_returns_OkObjectResult()
        {
            // Arrange
            var controller = new UserController(new HostingEnvironment(), _logger);

            // Act
            var result = controller.EndSession() as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
