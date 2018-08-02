using System.Collections.Generic;
using Xunit;
using SearchApi.Models;
using SearchApi.Repositories;
using Moq;
using SearchApi.Clients;

namespace SearchApi.Tests.Repositories
{
    public class EisRepositorySpec
    {
        [Fact]
        public async void EisRepository_GetEisCaseBenefits_returns_null_if_ESB_response_is_null()
        {
            // Arrange
            var caseModel = new CaseModel
            {
                ClientId = "1",
                Programs = new List<ProgramModel>()
            };

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<EisBenefitResponse>($"eis/client/{caseModel.ClientId}/case/{caseModel.CaseNumber}/benefit/0"))
                .ReturnsAsync((EisBenefitResponse)null);

            var eisRepository = new EisRepository(mock.Object);

            // Act
            var clientCase = await eisRepository.GetEisCaseBenefits(caseModel);

            // Assert
            Assert.Null(clientCase);
        }

        [Fact]
        public async void EisRepository_GetEisCaseBenefits_adds_issuances_to_clientCase()
        {
            // Arrange
            var caseModel = new CaseModel
            {
                ClientId = "1",
                CaseNumber = "2",
                Programs = new List<ProgramModel>{
                    new ProgramModel
                    {
                        ProgramName = "mockProgramName",
                        Issuances = new List<IssuanceModel>()
                    }
                }
            };

            var mock = new Mock<IEsbClient>();
            mock.Setup(m => m.GetAsync<EisBenefitResponse>($"eis/client/{caseModel.ClientId}/case/{caseModel.CaseNumber}/benefit/0"))
                .ReturnsAsync(new EisBenefitResponse
                {
                    CHES18F04 = new CHES18F04
                    {
                        outBenefitMonth = "0",
                        optOPTION = "mockOption",
                        optDESCRIPTION = "mockDescription",
                        outMedSubtype = "mockMedSubtype",
                        outIssuanceInd = "mockIssuanceInd"
                    }
                });

            mock.Setup(m => m.GetAsync<EisIssuanceResponse>($"eis/case/{caseModel.CaseNumber}/program/{caseModel.Programs[0].ProgramName}/benefit/0"))
                .ReturnsAsync(new EisIssuanceResponse
                {
                    cHES18F05 = new cHES18F05
                    {
                        outBenefitType1 = "bt1",
                        outBenefitType2 = "bt2",
                        outBenefitType3 = "bt3",
                        outBenefitType4 = "bt4",
                        outBenefitType5 = "bt5",
                        outIssueType1 = "it1",
                        outIssueType2 = "it2",
                        outIssueType3 = "it3",
                        outIssueType4 = "it4",
                        outIssueType5 = "it5",
                        outIssuedAmount1 = "ia1",
                        outIssuedAmount2 = "ia2",
                        outIssuedAmount3 = "ia3",
                        outIssuedAmount4 = "ia4",
                        outIssuedAmount5 = "ia5",
                        outIssuedDate1 = "20150101",
                        outIssuedDate2 = "20150101",
                        outIssuedDate3 = "20150101",
                        outIssuedDate4 = "20150101",
                        outIssuedDate5 = "20150101",
                        optDESCRIPTION = "mockDescription",
                        optOPTION = "mockOption",
                        outBenefitMonth = "mockBenefitMonth",
                        outCaseNumber = "mockCaseNumber",
                        outProgramType = "mockProgramType"
                    }
                });

            var eisRepository = new EisRepository(mock.Object);

            // Act
            var clientCase = await eisRepository.GetEisCaseBenefits(caseModel);

            // Assert
            // Assert.Equal(clientCase.ClientId, caseModel.ClientId);
            Assert.Equal(5, clientCase.Programs[0].Issuances.Count);
        }
    }
}