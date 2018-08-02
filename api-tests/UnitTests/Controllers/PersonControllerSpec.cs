using Xunit;
using SearchApi.Controllers;
using SearchApi.Clients;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SearchApi.Models;
using System.Collections.Generic;
using SearchApi.Repositories;
using Microsoft.Extensions.Logging;

namespace SearchApi.Tests.UnitTests.Controllers
{
    public class PersonControllerSpec
    {
        private Mock<IEsbClient> _mock;
        private ILogger<PersonController> _logger = new Mock<ILogger<PersonController>>().Object;
        private MciSearchResponse mciSearchResponse;
        private AriesCasesResponse ariesCasesResponse;
        private EisCasesResponse eisCasesResponse;
        private AriesBenefitResponse ariesBenefitResponse;
        private EisBenefitResponse eisBenefitResponse;
        private AriesAppResponse ariesAppResponse;

        public PersonControllerSpec()
        {
            mciSearchResponse = new MciSearchResponse();
            ariesCasesResponse = new AriesCasesResponse();
            eisCasesResponse = new EisCasesResponse();
            ariesBenefitResponse = new AriesBenefitResponse();
            eisBenefitResponse = new EisBenefitResponse();

            Registration registration = new Registration();
            registration.RegistrationName = "EIS_ID";
            registration.RegistrationValue = "mockRegistration";

            Registrations registrations = new Registrations();
            registrations.Registration = new List<Registration>();
            registrations.Registration.Add(registration);

            SearchResponsePerson person = new SearchResponsePerson();
            person.Registrations = registrations;

            mciSearchResponse.SearchResponsePerson = new List<SearchResponsePerson>();
            mciSearchResponse.SearchResponsePerson.Add(person);

            ariesCasesResponse.ARIESa3 = new ARIESa3()
            {
                optDESCRIPTION = "mockDescription",
                optOPTION = "00",
                outCaseNumber = "mockCaseNum",
                outCaseStatus = "mockCaseStatus",
                outHeadOfHouseholdFName = "mockFName",
                outHeadOfHouseholdID = "mockID",
                outHeadOfHouseholdLName = "mockLName",
                outHeadOfHouseholdMName = "mockMName",
                outProgramType = "mockProgramType",
            };

            ariesBenefitResponse.ARIESa4 = new ARIESa4()
            {
                optOPTION = "00",
                outBenMonth = "mockBenMonth",
                outClientID = "mockClientID",
                outMedEligCode = "mockMedCode",
                outMedIssueTypeDesc = "mockIssue",
                outMedSubType = "mockMedSubType"
            };

            eisCasesResponse.CHES18F03 = new CHES18F03()
            {
                optDESCRIPTION = "mockDescription",
                optOPTION = "00",
                outAFBenMonth = "mockBenMonth",
                outAFCaseNumber = "mockCaseNum",
                outAFPIFirst = "mockFirst",
                outAFPIInit = "mockInit",
                outAFPILast = "mockLast",
                outAFPIid = "mockId",
                outAFProgSubtype = "mockSubtype",
                outAFProgramStatus = "mockStatus",
                outNewClientNum = "mockClientNum",
                outAPPIInit = "mockPIInit",
                outDuplicateClient = "mockDuplicateClient",
                outFSPIInit = "mockFSPIInit",
                outGAPIInit = "mockGAPIInit",
                outGMPIInit = "mockGMPIInit",
                outIAPIInit = "mockIAPIInit",
                outInputClientNum = "mockInputClientNum",
                outMEPIInit = "mockMEPIInit"
            };

            eisBenefitResponse.CHES18F04 = new CHES18F04();
            eisBenefitResponse.CHES18F04.optOPTION = "00";

            ariesAppResponse = new AriesAppResponse
            {
                ARIESa7 = new ARIESa7
                {
                    optDESCRIPTION = "mockDescription",
                    optOPTION = "mockOption",
                    outApplication = new ApplicationList()
                    {
                        AriesApplication = new List<ApplicationInformation>()
                        {
                            new ApplicationInformation()
                            {
                                AppNumber = "mockAppNumber",
                                AppRecvdDate = "1/1/2018",
                                AppStatusCode = "mockStatusCode",
                                AppStatusDesc = "mockStatusDesc"
                            }
                        }
                    },
                    outIndvID = "mockIndvID"
                }
            };

            _mock = new Mock<IEsbClient>();
        }

        [Theory]
        [InlineData("")]
        [InlineData("111")]
        [InlineData("0000000000")]
        public async void AriesCases_should_not_accept_invalid_clientid(string clientid)
        {
            // Arrange
            var _mock = new Mock<IEsbClient>();
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetAriesCases(clientid) as ObjectResult;

            // Assert
            Assert.Equal(400, result.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData("111")]
        [InlineData("0000000000")]
        public async void EisCases_should_not_accept_invalid_clientid(string clientid)
        {
            // Arrange
            var _mock = new Mock<IEsbClient>();
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetEisCases(clientid) as ObjectResult;

            // Assert
            Assert.Equal(400, result.StatusCode);
        }

        [Fact]
        public async void AriesCases_should_not_return_error_for_empty_ESB_responses()
        {
            // Arrange
            // Here, we don't perform any mock setup to ensure the ESB response is empty
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetAriesCases("2400154032") as ObjectResult;

            // Assert
            // Verify there were no errors
            Assert.Equal(200, result.StatusCode);
        }

        [Fact]
        public async void EisCases_should_not_return_error_for_empty_ESB_responses()
        {
            // Arrange
            // Here, we don't perform any mock setup to ensure the ESB response is empty
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetEisCases("0600026712") as ObjectResult;

            // Assert
            // Verify there were no errors
            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [InlineData("2400154032")]
        [InlineData("2400154033")]
        [InlineData("2400154034")]
        public async void AriesCases_should_accept_valid_clientid(string clientid)
        {
            // Arrange
            _mock.Setup(m => m.GetAsync<AriesCasesResponse>($"aries/client/{clientid}/case")).ReturnsAsync(ariesCasesResponse);
            _mock.Setup(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{clientid}/benefit/0")).ReturnsAsync(ariesBenefitResponse);
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetAriesCases(clientid) as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [InlineData("0600026712")]
        [InlineData("0600026524")]
        [InlineData("0600026525")]
        public async void EisCases_should_accept_valid_clientid(string clientid)
        {
            // Arrange
            _mock.Setup(m => m.GetAsync<EisCasesResponse>($"eis/client/{clientid}/case")).ReturnsAsync(eisCasesResponse);
            _mock.Setup(m => m.GetAsync<EisBenefitResponse>($"eis/client/mockClientNum/case/mockCaseNum/benefit/0")).ReturnsAsync(eisBenefitResponse);
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetEisCases(clientid) as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async void GetPersonDetails_should_not_accept_blank_or_null_input(string registration)
        {
            // Arrange
            _mock.Setup(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>())).ReturnsAsync(mciSearchResponse);
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetPersonDetails(registration) as ObjectResult;

            // Assert
            Assert.Equal(400, result.StatusCode);
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Theory]
        [InlineData("0600026712")]
        [InlineData("0600026524")]
        [InlineData("2400154033")]
        [InlineData("2400154034")]
        public async void GetPersonDetails_should_not_error_on_no_mci_result(string clientid)
        {
            // Arrange
            IEsbClient _esbClient = new Mock<IEsbClient>().Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetPersonDetails(clientid) as ObjectResult;

            // Assert
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("0600026712")]
        public async void GetPersonDetails_should_return_OkObjectResult_with_eisid(string clientid)
        {
            // Arrange
            mciSearchResponse.SearchResponsePerson[0].Registrations.Registration[0].RegistrationName = "EIS_ID";
            mciSearchResponse.SearchResponsePerson[0].Registrations.Registration[0].RegistrationValue = clientid;
            _mock.Setup(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>())).ReturnsAsync(mciSearchResponse);
            _mock.Setup(m => m.GetAsync<EisCasesResponse>($"eis/client/{clientid}/case")).ReturnsAsync(eisCasesResponse);
            _mock.Setup(m => m.GetAsync<EisBenefitResponse>($"eis/client/mockClientNum/case/mockCaseNum/benefit/0")).ReturnsAsync(eisBenefitResponse);
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetPersonDetails(clientid) as ObjectResult;

            // Assert
            _mock.Verify(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>()), Times.AtLeastOnce());
            _mock.Verify(m => m.GetAsync<EisCasesResponse>($"eis/client/{clientid}/case"), Times.AtLeastOnce());
            _mock.Verify(m => m.GetAsync<EisBenefitResponse>($"eis/client/mockClientNum/case/mockCaseNum/benefit/0"), Times.AtLeastOnce());
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("2400154032")]
        public async void GetPersonDetails_should_return_OkObjectResult_with_ariesid(string clientid)
        {
            // Arrange
            mciSearchResponse.SearchResponsePerson[0].Registrations.Registration[0].RegistrationName = "ARIES_ID";
            mciSearchResponse.SearchResponsePerson[0].Registrations.Registration[0].RegistrationValue = clientid;
            _mock.Setup(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>())).ReturnsAsync(mciSearchResponse);
            _mock.Setup(m => m.GetAsync<AriesCasesResponse>($"aries/client/{clientid}/case")).ReturnsAsync(ariesCasesResponse);
            _mock.Setup(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{clientid}/benefit/0")).ReturnsAsync(ariesBenefitResponse);
            _mock.Setup(m => m.GetAsync<AriesAppResponse>($"aries/client/{clientid}/applications")).ReturnsAsync(ariesAppResponse);
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetPersonDetails(clientid) as ObjectResult;

            // Assert
            _mock.Verify(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>()), Times.AtLeastOnce());
            _mock.Verify(m => m.GetAsync<AriesCasesResponse>($"aries/client/{clientid}/case"), Times.AtLeastOnce());
            _mock.Verify(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{clientid}/benefit/0"), Times.AtLeastOnce());
            _mock.Verify(m => m.GetAsync<AriesAppResponse>($"aries/client/{clientid}/applications"), Times.AtLeastOnce());
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("0600026712")]
        public async void GetPersonDetails_should_handle_eisid_with_no_cases(string clientid)
        {
            // Arrange
            mciSearchResponse.SearchResponsePerson[0].Registrations.Registration[0].RegistrationName = "EIS_ID";
            mciSearchResponse.SearchResponsePerson[0].Registrations.Registration[0].RegistrationValue = clientid;
            _mock.Setup(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>())).ReturnsAsync(mciSearchResponse);
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetPersonDetails(clientid) as ObjectResult;

            // Assert
            _mock.Verify(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>()), Times.AtLeastOnce());
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }

        [Theory]
        [InlineData("2400154032")]
        public async void GetPersonDetails_should_handle_ariesid_with_no_cases(string clientid)
        {
            // Arrange
            mciSearchResponse.SearchResponsePerson[0].Registrations.Registration[0].RegistrationName = "ARIES_ID";
            mciSearchResponse.SearchResponsePerson[0].Registrations.Registration[0].RegistrationValue = clientid;
            _mock.Setup(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>())).ReturnsAsync(mciSearchResponse);
            _mock.Setup(m => m.GetAsync<AriesAppResponse>($"aries/client/{clientid}/applications")).ReturnsAsync(ariesAppResponse);
            IEsbClient _esbClient = _mock.Object;
            var ariesRepository = new AriesRepository(_esbClient);
            var eisRepository = new EisRepository(_esbClient);
            var mciRepository = new MciRepository(_esbClient);
            PersonController controller = new PersonController(ariesRepository, eisRepository, mciRepository, _logger);

            // Act
            var result = await controller.GetPersonDetails(clientid) as ObjectResult;

            // Assert
            _mock.Verify(m => m.PostAsync<MciSearchResponse>($"mci/person/search/", It.IsAny<object>()), Times.AtLeastOnce());
            _mock.Verify(m => m.GetAsync<AriesAppResponse>($"aries/client/{clientid}/applications"), Times.AtLeastOnce());
            Assert.Equal(200, result.StatusCode);
            Assert.IsType<OkObjectResult>(result);
        }
    }
}
