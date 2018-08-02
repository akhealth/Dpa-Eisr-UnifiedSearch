using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using SearchApi.Clients;
using SearchApi.Models;

namespace SearchApi.Repositories
{
    public interface IEisRepository
    {
        Task<CaseModel> GetEisCaseBenefits(CaseModel clientCase);
        Task<IEnumerable<CaseModel>> GetFormattedEisCases(string clientId);
    }

    public class EisRepository : BaseRepository, IEisRepository
    {
        public EisRepository(IEsbClient esbClient) : base(esbClient) { }

        /// <summary>
        /// Retrieve the benefits and issuances associated with a certain client on a certain case.
        /// </summary>
        /// <remarks>
        /// Currently returns only the most recent benefit month.
        /// </remarks>
        public async Task<CaseModel> GetEisCaseBenefits(CaseModel clientCase)
        {
            var response = await _esbClient.GetAsync<EisBenefitResponse>($"eis/client/{clientCase.ClientId}/case/{clientCase.CaseNumber}/benefit/0");

            if (response == null)
            {
                return null;
            }

            var benefit = response.CHES18F04;
            var issuanceTasks = new List<Task>();

            foreach (var program in clientCase.Programs)
            {
                // the benefit response only applies to the ME program
                if (benefit.optOPTION.Equals("00") && program.ProgramName == "ME")
                {
                    program.BenefitsMonth = VerifyResponseData(benefit.outBenefitMonth, program.BenefitsMonth);
                    program.EligibilityCode = VerifyResponseData(benefit.outEligibilityCode, program.EligibilityCode);
                    program.MedicaidSubType = VerifyResponseData(benefit.outMedSubtype, program.MedicaidSubType);
                }

                // for the eis issuance endpoint, benefit/0 works to retrieve the latest populated benefit month
                issuanceTasks.Add(_esbClient.GetAsync<EisIssuanceResponse>($"eis/case/{clientCase.CaseNumber}/program/{program.ProgramName}/benefit/0")
                    .ContinueWith(issuanceResult =>
                    {
                        var issuanceResponse = issuanceResult.Result;
                        // The optOption isn't always "00" in a correct response, so instead use the benefit month, which returns "0" if not found
                        if (issuanceResponse == null || issuanceResponse.cHES18F05.outBenefitMonth == "0")
                        {
                            return;
                        }

                        for (int i = 0; i < issuanceResponse.cHES18F05.BenefitTypes.Count(); i++)
                        {
                            IssuanceModel issuance = new IssuanceModel();
                            issuance.BenefitType = VerifyResponseData(issuanceResponse.cHES18F05.BenefitTypes.ElementAt(i), issuance.BenefitType);
                            issuance.IssuanceType = VerifyResponseData(issuanceResponse.cHES18F05.IssuanceTypes.ElementAt(i), issuance.IssuanceType);
                            issuance.IssuanceAmount = VerifyResponseData(issuanceResponse.cHES18F05.IssuanceAmounts.ElementAt(i), issuance.IssuanceAmount);
                            DateTime outDate;
                            DateTime.TryParseExact(issuanceResponse.cHES18F05.IssuanceDates.ElementAt(i), "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out outDate);
                            issuance.IssuanceDate = outDate;
                            program.Issuances.Add(issuance);
                        }
                        program.Issuances.Sort((x, y) => DateTime.Compare(y.IssuanceDate, x.IssuanceDate));
                    })
                );
            }

            Task.WaitAll(issuanceTasks.ToArray());

            // Sort programs by issuance date, with programs that don't have an issuance first
            clientCase.Programs = clientCase.Programs.OrderByDescending(p => p.Issuances.Any() ? p.Issuances.FirstOrDefault().IssuanceDate : DateTime.MaxValue).ToList();

            return clientCase;
        }

        /// <summary>
        /// Retrieve the eis cases associated with a certain client.
        /// </summary>
        public async Task<IEnumerable<CaseModel>> GetFormattedEisCases(string clientId)
        {
            var response = await _esbClient.GetAsync<EisCasesResponse>($"eis/client/{clientId}/case");

            if (response == null || !response.CHES18F03.optOPTION.Equals("00"))
            {
                return null;
            }

            var cases = response.CHES18F03.CaseList.ToList();

            return cases;
        }
    }
}