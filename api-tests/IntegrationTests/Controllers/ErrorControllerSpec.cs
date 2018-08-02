using Xunit;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SearchApi.Tests.IntegrationTests.Controllers
{
    public class ErrorControllerSpec
    {
        private readonly TestServer _server;
        private readonly HttpClient _client;

        public ErrorControllerSpec()
        {
            // Arrange
            _server = new TestServer(new WebHostBuilder().UseStartup<Startup>().UseEnvironment("development"));
            _client = _server.CreateClient();
        }

        [Fact]
        public async Task ErrorController_should_return_bad_request()
        {
            // Act
            var response = await _client.GetAsync("error");

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task ErrorController_should_show_stack_in_development()
        {
            // Arrange
            var respFormat = new
            {
                success = false,
                data = new
                {
                    error = "",
                    query = new List<string>(),
                    stack = new List<string>(),
                    headers = new { }
                }
            };

            // Act
            var response = await _client.GetAsync("error");
            var json = JsonConvert.DeserializeAnonymousType(await response.Content.ReadAsStringAsync(), respFormat);

            // Assert
            Assert.NotEmpty(json.data.stack);
        }
    }
}
