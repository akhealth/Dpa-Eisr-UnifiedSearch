using System.Collections.Generic;
using Xunit;
using SearchApi.Models;
using SearchApi.Repositories;
using Moq;
using SearchApi.Clients;

namespace SearchApi.Tests.Repositories
{
    public class MciRepositorySpec
    {
        [Fact]
        public async void MciRepository_GetMci_returns_null_if_ESB_response_is_null()
        {
            // Arrange
            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>()))
                .ReturnsAsync((MciSearchResponse)null);

            var mciRepository = new MciRepository(mock.Object);

            // Act
            var searchResponse = await mciRepository.GetMci(null, null, null);

            // Assert
            Assert.Null(searchResponse);
        }

        [Fact]
        public async void MciRepository_GetMci_returns_SearchResponsePerson()
        {
            // Arrange
            Registration registration = new Registration();
            registration.RegistrationName = "EIS_ID";
            registration.RegistrationValue = "mockRegistration";

            Registrations registrations = new Registrations();
            registrations.Registration = new List<Registration>();
            registrations.Registration.Add(registration);

            SearchResponsePerson person = new SearchResponsePerson();
            person.Registrations = registrations;

            MciSearchResponse mciSearchResponse = new MciSearchResponse();
            mciSearchResponse.SearchResponsePerson = new List<SearchResponsePerson>();
            mciSearchResponse.SearchResponsePerson.Add(person);

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>()))
                .ReturnsAsync(mciSearchResponse);

            var mciRepository = new MciRepository(mock.Object);

            // Act
            var searchResponse = await mciRepository.GetMci(null, null, registration.RegistrationValue);

            // Assert
            Assert.Equal(searchResponse[0].Registrations.Registration[0].RegistrationValue, registration.RegistrationValue);
        }

        [Theory]
        [InlineData(null, null, null)]
        [InlineData(null, null, "")]
        [InlineData("", "", "")]
        public void MciRepository_ValidateMCIInput_returns_invalid_for_null_or_empty_inputs(string firstName, string lastName, string registration)
        {
            // Arrange
            var mock = new Mock<IEsbClient>();
            var mciRepository = new MciRepository(mock.Object);

            // Act
            var result = mciRepository.ValidateMciInput(firstName, lastName, registration);

            // Assert
            Assert.NotNull(result);
        }

        [Theory]
        [InlineData("001010001")]
        [InlineData("665010001")]
        [InlineData("667010001")]
        [InlineData("899010001")]
        public void MciRepository_ValidateMCIInput_returns_null_for_valid_ssns(string registration)
        {
            // Arrange
            var mock = new Mock<IEsbClient>();
            var mciRepository = new MciRepository(mock.Object);

            // Act
            var result = mciRepository.ValidateMciInput(null, null, registration);

            // Assert
            Assert.Null(result);
        }
    }
}