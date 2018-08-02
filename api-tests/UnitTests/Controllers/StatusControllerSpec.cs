using Xunit;
using SearchApi.Controllers;
using Microsoft.Extensions.Configuration;
using SearchApi.Clients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SearchApi.Models;
using SearchApi.Repositories;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace SearchApi.Tests.UnitTests.Controllers
{
    public class StatusControllerSpec
    {
        private ILogger<StatusController> _logger = new Mock<ILogger<StatusController>>().Object;
        private IHostingEnvironment _environment = new Mock<IHostingEnvironment>().Object;

        [Fact]
        public async void GetStatus_should_return_OkObjectResult()
        {
            // Arrange
            var mockEsbClient = new Mock<IEsbClient>();
            mockEsbClient.Setup(m => m.PostAsync<MciSearchResponse>("mci/person/search/", new { }))
                .ReturnsAsync(new MciSearchResponse());

            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var mciRepository = new MciRepository(mockEsbClient.Object);
            var controller = new StatusController(configuration, _environment, mciRepository, _logger);

            // Act
            var result = await controller.GetStatus() as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
