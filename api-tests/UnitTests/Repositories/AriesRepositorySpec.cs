using System.Collections.Generic;
using Xunit;
using SearchApi.Models;
using SearchApi.Repositories;
using Moq;
using SearchApi.Clients;
using System.Linq;
using System;

namespace SearchApi.Tests.Repositories
{
    public class AriesRepositorySpec
    {
        [Fact]
        public async void AriesRepository_GetAriesCaseBenefits_returns_null_if_ESB_response_is_null()
        {
            // Arrange
            var caseModel = new CaseModel
            {
                ClientId = "1",
                Programs = new List<ProgramModel>()
            };

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{caseModel.ClientId}/benefit/0"))
                .ReturnsAsync((AriesBenefitResponse)null);

            var ariesRepository = new AriesRepository(mock.Object);

            // Act
            var clientCase = await ariesRepository.GetAriesCaseBenefits(caseModel);

            // Assert
            Assert.Null(clientCase);
        }

        [Fact]
        public async void AriesRepository_GetAriesCaseBenefits_returns_clientCase()
        {
            // Arrange
            var caseModel = new CaseModel
            {
                ClientId = "1",
                Programs = new List<ProgramModel>()
            };

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{caseModel.ClientId}/benefit/0"))
                .ReturnsAsync(new AriesBenefitResponse
                {
                    ARIESa4 = new ARIESa4
                    {
                        optOPTION = "mockOPTION",
                        optDESCRIPTION = "mockDescription"
                    }
                });

            var ariesRepository = new AriesRepository(mock.Object);

            // Act
            var clientCase = await ariesRepository.GetAriesCaseBenefits(caseModel);

            // Assert
            Assert.Equal(clientCase.ClientId, caseModel.ClientId);
        }

        [Fact]
        public async void AriesRepository_GetAriesCaseBenefits_adds_issuances_to_clientCase()
        {
            // Arrange
            var caseModel = new CaseModel
            {
                ClientId = "1",
                CaseNumber = "2",
                Programs = new List<ProgramModel>{
                    new ProgramModel
                    {
                        ProgramName = "ME",
                        Issuances = new List<IssuanceModel>()
                    }
                }
            };

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{caseModel.ClientId}/benefit/0"))
                .ReturnsAsync(new AriesBenefitResponse
                {
                    ARIESa4 = new ARIESa4
                    {
                        optOPTION = "00",
                        outBenMonth = "3"
                    }
                });
            mock.Setup(m => m.GetAsync<AriesIssuanceResponse>($"aries/case/{caseModel.CaseNumber}/benefit/{3}"))
                .ReturnsAsync(new AriesIssuanceResponse
                {
                    ARIESa6 = new ARIESa6
                    {
                        outIssuance_dt = "01/01/2001",
                        optDESCRIPTION = "mockDescription",
                        optOPTION = "00",
                        outBenefitMonth = "mockBenefitMonth",
                        outCaseNum = "mockCaseNum"
                    }
                });

            var ariesRepository = new AriesRepository(mock.Object);

            // Act
            var clientCase = await ariesRepository.GetAriesCaseBenefits(caseModel);

            // Assert
            Assert.Equal(clientCase.ClientId, caseModel.ClientId);
            Assert.Single(clientCase.Programs[0].Issuances);
        }

        [Fact]
        public async void AriesRepository_populates_program_information()
        {
            // Arrange
            var caseModel = new CaseModel
            {
                ClientId = "1",
                Programs = new List<ProgramModel>{
                    new ProgramModel
                    {
                        ProgramName = "ME",
                        Issuances = new List<IssuanceModel>()
                    }
                }
            };

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{caseModel.ClientId}/benefit/0"))
                    .ReturnsAsync(new AriesBenefitResponse
                    {
                        ARIESa4 = new ARIESa4
                        {
                            optOPTION = "00",
                            outBenMonth = "mockBenMonth",
                            outMedEligCode = "mockMedEligCode",
                            outMedSubType = "mockMedSubType",
                            outMedIssueTypeDesc = "mockMedIssueTypeDesc"
                        }
                    });

            var ariesRepository = new AriesRepository(mock.Object);

            // Act
            var clientCase = await ariesRepository.GetAriesCaseBenefits(caseModel);

            // Assert
            Assert.Equal(clientCase.ClientId, caseModel.ClientId);
            Assert.NotNull(clientCase.Programs[0].BenefitsMonth);
        }

        [Fact]
        public async void AriesRepository_GetAriesApplication_returns_application()
        {
            // Arrange
            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<AriesAppResponse>($"aries/client/1/applications"))
                .ReturnsAsync(new AriesAppResponse
                {
                    ARIESa7 = new ARIESa7
                    {
                        optOPTION = "00",
                        outApplication = new ApplicationList()
                        {
                            AriesApplication = new List<ApplicationInformation>()
                            {
                                new ApplicationInformation()
                                {
                                    AppNumber = "12345",
                                    AppRecvdDate = "mockReceivedDate",
                                    AppStatusCode = "mockStatusCode",
                                    AppStatusDesc = "mockStatusDesc"
                                }
                            }
                        }
                    }
                });

            var ariesRepository = new AriesRepository(mock.Object);

            // Act
            var application = await ariesRepository.GetAriesApplications("1");

            // Assert
            Assert.Equal("12345", application.First().ApplicationNumber);
        }

        [Fact]
        public async void AriesRepository_GetAriesApplication_returns_null_when_optOPTION_is_NOT_00()
        {
            // Arrange
            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<AriesAppResponse>($"aries/client/1/applications"))
                .ReturnsAsync(new AriesAppResponse
                {
                    ARIESa7 = new ARIESa7
                    {
                        optOPTION = "99",
                        outApplication = new ApplicationList()
                        {
                            AriesApplication = new List<ApplicationInformation>()
                            {
                                new ApplicationInformation()
                                {
                                    AppNumber = "12345",
                                    AppRecvdDate = "mockReceivedDate",
                                    AppStatusCode = "mockStatusCode",
                                    AppStatusDesc = "mockStatusDesc"
                                }
                            }
                        }
                    }
                });

            var ariesRepository = new AriesRepository(mock.Object);

            // Act
            var application = await ariesRepository.GetAriesApplications("1");

            // Assert
            Assert.Null(application);
        }

        [Fact]
        public async void AriesRepository_handles_invalid_certification_dates()
        {
            // Arrange
            var caseModel = new CaseModel
            {
                ClientId = "1",
                Programs = new List<ProgramModel>{
                    new ProgramModel
                    {
                        ProgramName = "ME",
                        Issuances = new List<IssuanceModel>()
                    }
                }
            };

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{caseModel.ClientId}/benefit/0"))
                    .ReturnsAsync(new AriesBenefitResponse
                    {
                        ARIESa4 = new ARIESa4
                        {
                            optOPTION = "00",
                            outBenMonth = "mockBenMonth",
                            outMedEligCode = "mockMedEligCode",
                            outMedSubType = "mockMedSubType",
                            outMedIssueTypeDesc = "mockMedIssueTypeDesc",
                            outEligBeginDate = "0",
                            outEligEndDate = "0"
                        }
                    });

            var ariesRepository = new AriesRepository(mock.Object);

            // Act
            var clientCase = await ariesRepository.GetAriesCaseBenefits(caseModel);

            // Assert
            Assert.Equal(clientCase.ClientId, caseModel.ClientId);
            Assert.Equal("", clientCase.Programs[0].CertificationStart);
            Assert.Equal("", clientCase.Programs[0].CertificationEnd);
        }

        [Fact]
        public async void AriesRepository_handles_valid_certification_dates()
        {
            // Arrange
            string dateString = "01-01-1900";
            var caseModel = new CaseModel
            {
                ClientId = "1",
                Programs = new List<ProgramModel>{
                    new ProgramModel
                    {
                        ProgramName = "ME",
                        Issuances = new List<IssuanceModel>()
                    }
                }
            };

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<AriesBenefitResponse>($"aries/client/{caseModel.ClientId}/benefit/0"))
                    .ReturnsAsync(new AriesBenefitResponse
                    {
                        ARIESa4 = new ARIESa4
                        {
                            optOPTION = "00",
                            outBenMonth = "mockBenMonth",
                            outMedEligCode = "mockMedEligCode",
                            outMedSubType = "mockMedSubType",
                            outMedIssueTypeDesc = "mockMedIssueTypeDesc",
                            outEligBeginDate = dateString,
                            outEligEndDate = dateString
                        }
                    });

            var ariesRepository = new AriesRepository(mock.Object);

            // Act
            var clientCase = await ariesRepository.GetAriesCaseBenefits(caseModel);

            // Assert
            Assert.Equal(clientCase.ClientId, caseModel.ClientId);
            Assert.Equal(DateTime.Parse(dateString).ToShortDateString(), clientCase.Programs[0].CertificationStart);
            Assert.Equal(DateTime.Parse(dateString).ToShortDateString(), clientCase.Programs[0].CertificationEnd);
        }
    }
}