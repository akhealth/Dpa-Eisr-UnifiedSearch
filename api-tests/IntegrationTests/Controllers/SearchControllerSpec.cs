using System.Collections.Generic;
using Xunit;
using SearchApi.Controllers;
using Microsoft.Extensions.Configuration;
using SearchApi.Clients;
using SearchApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace SearchApi.Tests.IntegrationTests.Controllers
{
    public class SearchControllerSpec
    {
        private ILogger<SearchController> _logger = new Mock<ILogger<SearchController>>().Object;

        private static IConfiguration configuration;

        private static IEsbClient _esbClient;

        private static IMciRepository _mciRepository;

        public SearchControllerSpec()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            configuration = builder.Build();
            _esbClient = new EsbClient(configuration);
            _mciRepository = new MciRepository(_esbClient);
        }

        private static IDictionary<string, string> _fixtures = new Dictionary<string, string>
        {
            // input strings that are too long for mci
            ["mci_fname_too_long"] = new string('*', MciRepository.FIRSTNAME_MAX_LENGTH + 1),
            ["mci_lname_too_long"] = new string('*', MciRepository.LASTNAME_MAX_LENGTH + 1),
            ["mci_reg_too_long"] = new string('*', MciRepository.REGISTRATION_MAX_LENGTH + 1),

            // mci registration containing invalid format (should match regexes)
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
        public async void Mci_search_should_not_accept_invalid_input(string firstName, string lastName, string registration)
        {
            // Arrange
            var controller = new SearchController(_mciRepository, _logger);

            // Act
            var response = await controller.GetMci(firstName, lastName, registration) as ObjectResult;

            // Assert
            Assert.Equal(400, response.StatusCode);
        }
    }
}