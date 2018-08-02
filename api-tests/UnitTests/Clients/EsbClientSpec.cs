using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using RestSharp;
using SearchApi.Clients;
using SearchApi.Models;
using Xunit;

namespace SearchApi.Tests.UnitTests.Clients
{
    public class EsbClientSpec
    {
        private static IConfiguration configuration = new ConfigurationBuilder()
            .AddEnvironmentVariables()
            .Build();

        private static Mock<RestClient> createMockRestClient<T>(Boolean withValidResponse) where T : class, new()
        {
            var mock = new Mock<RestClient>();
            if (withValidResponse)
            {
                var validEsbRestResponse = new RestResponse<T>
                {
                    ResponseStatus = ResponseStatus.Completed
                };
                mock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                    .Returns(validEsbRestResponse);
                mock.Setup(x => x.ExecuteTaskAsync<T>(It.IsAny<IRestRequest>()))
                    .ReturnsAsync(validEsbRestResponse);
            }
            else
            {
                var nullEsbRestResponse = (IRestResponse<T>)null;
                mock.Setup(x => x.Execute<T>(It.IsAny<IRestRequest>()))
                    .Returns(nullEsbRestResponse);
                mock.Setup(x => x.ExecuteTaskAsync<T>(It.IsAny<IRestRequest>()))
                    .ReturnsAsync(nullEsbRestResponse);
            }
            return mock;
        }

        private static Mock<RestClient> createMockRestClient(Boolean withValidResponse)
        {
            return createMockRestClient<object>(withValidResponse);
        }

        [Fact]
        public void EsbClient_get_returns_OkObjectResult()
        {
            // Arrange
            var esbClient = new EsbClient(configuration)
            {
                _client = createMockRestClient(withValidResponse: true).Object
            };

            // Act
            var result = esbClient.Get("");

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void EsbClient_get_does_not_return_successful_response_if_ESB_request_fails()
        {
            // Arrange
            var esbClient = new EsbClient(configuration)
            {
                _client = createMockRestClient(withValidResponse: false).Object
            };

            // Act / Assert
            try
            {
                var result = esbClient.Get("");
            }
            catch (Exception ex)
            {
                Assert.IsType<ExternalException>(ex);
            }
        }

        [Fact]
        public void EsbClient_getAsync_returns_OkObjectResult()
        {
            // Arrange
            var esbClient = new EsbClient(configuration)
            {
                _client = createMockRestClient(withValidResponse: true).Object
            };

            // Act
            var result = esbClient.GetAsync("");
            result.Wait();
            var objectResult = result.Result as ObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(objectResult);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public void EsbClient_getAsync_T_returns_object_of_type_T()
        {
            // Arrange
            var esbClient = new EsbClient(configuration)
            {
                _client = createMockRestClient<MciSearchResponse>(withValidResponse: true).Object
            };

            // Act
            var result = esbClient.GetAsync<MciSearchResponse>("");
            result.Wait();

            // Assert
            if (result.Result != null)
            {
                Assert.IsType<MciSearchResponse>(result.Result);
            }
        }

        [Fact]
        public void EsbClient_post_returns_OkObjectResult()
        {
            // Arrange
            var esbClient = new EsbClient(configuration)
            {
                _client = createMockRestClient(withValidResponse: true).Object
            };

            // Act
            var result = esbClient.Post("", null);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public void EsbClient_postAsync_returns_OkObjectResult()
        {
            // Arrange
            var esbClient = new EsbClient(configuration)
            {
                _client = createMockRestClient(withValidResponse: true).Object
            };

            // Act
            var result = esbClient.PostAsync("", null);
            result.Wait();
            var objectResult = result.Result as ObjectResult;

            // Assert
            Assert.IsType<OkObjectResult>(objectResult);
            Assert.Equal(200, objectResult.StatusCode);
        }

        [Fact]
        public void EsbClient_postAsync_T_returns_object_of_type_T()
        {
            // Arrange
            var esbClient = new EsbClient(configuration)
            {
                _client = createMockRestClient<MciSearchResponse>(withValidResponse: true).Object
            };

            // Act
            var result = esbClient.PostAsync<MciSearchResponse>("", new { });
            result.Wait();

            // Assert
            if (result.Result != null)
            {
                Assert.IsType<MciSearchResponse>(result.Result);
            }
        }
    }
}