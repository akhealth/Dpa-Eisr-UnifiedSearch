using Xunit;
using SearchApi.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace SearchApi.Tests.Controllers
{
    public class BaseControllerSpec
    {
        private ILogger<BaseController> _logger = new Mock<ILogger<BaseController>>().Object;

        [Fact]
        public void BaseController_returns_OkObjectResult()
        {
            // Arrange
            var controller = new BaseController(_logger);

            // Act
            var result = controller.Ok();

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void BaseController_returns_OkObjectResult_with_payload()
        {
            // Arrange
            var controller = new BaseController(_logger);

            // Act
            var result = controller.Ok("test");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, result.StatusCode);
            Assert.Equal("test", GetRequestObjectData<string>(result));
        }

        [Fact]
        public void BaseController_returns_BadRequestObjectResult()
        {
            // Arrange
            var controller = new BaseController(_logger);

            // Act
            var result = controller.BadRequest();

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public void BaseController_returns_BadRequestObjectResult_with_payload()
        {
            // Arrange
            var controller = new BaseController(_logger);

            // Act
            var result = controller.BadRequest("test");

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, result.StatusCode);
            Assert.Equal("test", GetRequestObjectData<string>(result));
        }

        public T GetRequestObjectData<T>(object o)
        {
            var value = GetPropertyValue(o, "Value");
            var data = GetPropertyValue(value, "data");
            return (T)data;
        }

        public object GetPropertyValue(object o, string propertyName)
        {
            return GetPropertyValue<object>(o, propertyName);
        }

        public T GetPropertyValue<T>(object o, string propertyName)
        {
            return (T)o.GetType()
                .GetProperty(propertyName)
                .GetValue(o, null);
        }
    }
}
