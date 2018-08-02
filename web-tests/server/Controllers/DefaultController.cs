using System;
using Xunit;
using SearchWeb.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SearchWeb.Tests.Controllers
{
    public class SearchControllerTests
    {
        [Fact]
        public void Web_default_controller_exists()
        {
            // Arrange
            var controller = new DefaultController();

            // Act
            var result = controller.Index();

            // Assert
            Assert.IsType<VirtualFileResult>(result);
        }

        [Fact]
        public void Web_default_error_controller_exists()
        {
            // Arrange
            var controller = new DefaultController();

            // Act
            var result = controller.Error(null);

            // Assert
            Assert.IsType<VirtualFileResult>(result);
        }

        [Fact]
        public void Web_default_error_controller_404_http_status_code()
        {
            // Arrange
            var controller = new DefaultController();

            // Act
            var result = controller.Error(404);

            // Assert
            Assert.IsType<VirtualFileResult>(result);
        }

        [Fact]
        public void Web_default_error_controller_handles_other_http_status_codes()
        {
            // Arrange
            var controller = new DefaultController();

            // Act
            var result = controller.Error(500);

            // Assert
            Assert.IsType<VirtualFileResult>(result);
        }
    }
}