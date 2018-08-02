using System.Collections.Generic;
using Xunit;
using SearchApi.Controllers;
using SearchApi.Clients;
using SearchApi.Models;
using SearchApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Moq;
using Microsoft.Extensions.Logging;

namespace SearchApi.Tests.UnitTests.Controllers
{
    public class SearchControllerSpec
    {
        private ILogger<SearchController> _logger = new Mock<ILogger<SearchController>>().Object;

        private static IDictionary<string, string> _fixtures = new Dictionary<string, string>
        {
            // input strings that are too long for mci
            ["mci_fname_too_long"] = new string('*', MciRepository.FIRSTNAME_MAX_LENGTH + 1),
            ["mci_lname_too_long"] = new string('*', MciRepository.LASTNAME_MAX_LENGTH + 1),
            ["mci_reg_too_long"] = new string('*', MciRepository.REGISTRATION_MAX_LENGTH + 1),

            // mci registration containing invalid characters (should be only numbers)
            ["mci_reg_invalid_char"] = "invalid reg",

            // mci name allows only certain characters, this name contains forbidden characters
            ["mci_name_invalid_char"] = "name with invalid chars {}!@",

            // input id that is too long for eis
            ["eis_id_too_long"] = new string('*', MciRepository.CLIENTID_MAX_LENGTH + 1),

            // eis id containing invalid characters (should be only numbers)
            ["eis_id_invalid_char"] = "invalid clientid"
        };

        public static IEnumerable<object[]> InvalidInputs =>
            new List<object[]>
            {
                new object[] { _fixtures["mci_fname_too_long"], "", "" },
                new object[] { "", _fixtures["mci_lname_too_long"], "" },
                new object[] { "", "", _fixtures["mci_reg_too_long"] },
                new object[] {
                    _fixtures["mci_fname_too_long"],
                    _fixtures["mci_lname_too_long"],
                    _fixtures["mci_reg_too_long"]
                },
                new object[] { _fixtures["mci_name_invalid_char"], "", "" },
                new object[] { "", _fixtures["mci_name_invalid_char"], "" },
                new object[] { "", "", _fixtures["mci_reg_invalid_char"] }
            };

        [Theory]
        [MemberData(nameof(InvalidInputs))]
        public async void MCI_search_should_not_accept_invalid_input(string firstName, string lastName, string registration)
        {
            // Arrange
            var _mock = new Mock<IEsbClient>();
            _mock.Setup(m => m.PostAsync<MciSearchResponse>("", new { }))
                .ReturnsAsync(new MciSearchResponse());

            IEsbClient _esbClient = _mock.Object;
            var mciRepository = new MciRepository(_esbClient);
            var controller = new SearchController(mciRepository, _logger);

            // Act
            var response = await controller.GetMci(firstName, lastName, registration) as ObjectResult;

            // Assert
            Assert.Equal(400, response.StatusCode);
        }

        public static IEnumerable<object[]> ValidInputs =>
            new List<object[]>
            {
                new object[] { "firstname", "", "" },
                new object[] { "", "lastname", "" },
                new object[] { "", "", "111111111" },
                new object[] { "firstname", "lastname", "0600039718" }
            };

        [Theory]
        [MemberData(nameof(ValidInputs))]
        public async Task MCI_search_works_with_valid_input(string firstName, string lastName, string registration)
        {
            // Arrange
            var mock = new Mock<IEsbClient>();

            var mockResponse = new MciSearchResponse
            {
                SearchResponsePerson = new List<SearchResponsePerson>
                {
                    new SearchResponsePerson
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        Registrations = new Registrations
                        {
                            Registration = new List<Registration>
                            {
                                new Registration {
                                    RegistrationName = "EIS_ID"
                                }
                            }
                        }
                    }
                }
            };

            mock.Setup(m => m.PostAsync<MciSearchResponse>("mci/person/search/", It.IsAny<object>()))
                .ReturnsAsync(mockResponse);

            IEsbClient _esbClient = mock.Object;
            var mciRepository = new MciRepository(_esbClient);
            var controller = new SearchController(mciRepository, _logger);

            // Act
            var response = await controller.GetMci(firstName, lastName, registration) as ObjectResult;

            // Assert
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task MCI_search_returns_Ok_when_ESB_result_is_null()
        {
            // Arrange
            var mock = new Mock<IEsbClient>();

            mock.Setup(m => m.PostAsync<MciSearchResponse>("mci/person/search/", It.IsAny<object>()))
                .ReturnsAsync((MciSearchResponse)null);

            IEsbClient _esbClient = mock.Object;
            var mciRepository = new MciRepository(_esbClient);
            var controller = new SearchController(mciRepository, _logger);

            // Act
            var response = await controller.GetMci("firstName", "lastName", "0600039718") as ObjectResult;

            // Assert
            Assert.Equal(200, response.StatusCode);
        }

        [Fact]
        public async Task MCI_search_returns_Ok_when_SearchResponsePerson_is_null()
        {
            // Arrange
            var mock = new Mock<IEsbClient>();

            mock.Setup(m => m.PostAsync<MciSearchResponse>("mci/person/search/", It.IsAny<object>()))
                .ReturnsAsync(new MciSearchResponse
                {
                    SearchResponsePerson = null
                }); ;

            IEsbClient _esbClient = mock.Object;
            var mciRepository = new MciRepository(_esbClient);
            var controller = new SearchController(mciRepository, _logger);

            // Act
            var response = await controller.GetMci("firstName", "lastName", "0600039718") as ObjectResult;

            // Assert
            Assert.Equal(200, response.StatusCode);
        }
    }
}
