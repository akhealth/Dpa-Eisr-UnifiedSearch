using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SearchApi.Clients;
using SearchApi.Models;

namespace SearchApi.Repositories
{
    public interface IAriesRepository
    {
        Task<CaseModel> GetAriesCaseBenefits(CaseModel clientCase);
        Task<IEnumerable<ApplicationModel>> GetAriesApplications(string clientId);
        Task<IEnumerable<CaseModel>> GetFormattedAriesCases(string clientId);
    }

    public class AriesRepository : BaseRepository, IAriesRepository
    {
        public AriesRepository(IEsbClient esbClient) : base(esbClient) { }

        /// <summary>
        /// Retrieve the aries benefits and issuances associated with a certain client on a certain case.
        /// </summary>
        /// <remarks>
        /// Currently returns only the most recent benefit month.
        /// </remarks>
        public async Task<CaseModel> GetAriesCaseBenefits(CaseModel clientCase)
        {
            var response = await _esbClient.GetAsync<AriesBenefitResponse>($"aries/client/{clientCase.ClientId}/benefit/0");

            if (response == null)
            {
                return null;
            }

            var benefit = response.ARIESa4;
            var issuanceTasks = new List<Task>();

            foreach (var program in clientCase.Programs)
            {
                if (!benefit.optOPTION.Equals("00") || program.ProgramName != "ME")
                {
                    // if the benefit endpoint doesn't return valid, we shouldn't check for issuances on that program
                    continue;
                }

                program.BenefitsMonth = VerifyResponseData(benefit.outBenMonth, program.BenefitsMonth);
                program.EligibilityCode = VerifyResponseData(benefit.outMedEligCode, program.EligibilityCode);
                program.MedicaidSubType = VerifyResponseData(benefit.outMedSubType, program.MedicaidSubType);
                program.ProgramSubtype = VerifyResponseData(benefit.outMedIssueTypeDesc, program.ProgramSubtype);
                program.CertificationStart = VerifyResponseData(benefit.outEligBeginDate, "");
                program.CertificationEnd = VerifyResponseData(benefit.outEligEndDate, "");

                if (program.CertificationStart == "0")
                {
                    program.CertificationStart = "";
                }
                else if (program.CertificationStart != "")
                {
                    DateTime certStart;
                    DateTime.TryParse(program.CertificationStart, out certStart);
                    program.CertificationStart = certStart.ToShortDateString();
                }

                if (program.CertificationEnd == "0")
                {
                    program.CertificationEnd = "";
                }
                else if (program.CertificationEnd != "")
                {
                    DateTime certEnd;
                    DateTime.TryParse(program.CertificationEnd, out certEnd);
                    program.CertificationEnd = certEnd.ToShortDateString();
                }

                // for the aries issuance endpoint, benefit/0 doesn't return the most recent month, so instead use the
                // benefit month returned by the benefit endpoint
                issuanceTasks.Add(_esbClient.GetAsync<AriesIssuanceResponse>($"aries/case/{clientCase.CaseNumber}/benefit/{program.BenefitsMonth}")
                    .ContinueWith(issuanceResult =>
                    {
                        var issuanceResponse = issuanceResult.Result;
                        // this relies on the optOption in the issuanceResponse always being "00" if the result is valid
                        if (issuanceResponse == null || issuanceResponse.ARIESa6 == null || !issuanceResponse.ARIESa6.optOPTION.Equals("00"))
                        {
                            return;
                        }

                        IssuanceModel issuance = new IssuanceModel();
                        DateTime outDate;
                        DateTime.TryParse(issuanceResponse.ARIESa6.outIssuance_dt, out outDate);
                        issuance.IssuanceDate = outDate;
                        program.Issuances.Add(issuance);
                    })
                );
            }

            Task.WaitAll(issuanceTasks.ToArray());

            // Sort programs by issuance date, with programs that don't have an issuance first
            clientCase.Programs = clientCase.Programs.OrderByDescending(p => p.Issuances.Any() ? p.Issuances.FirstOrDefault().IssuanceDate : DateTime.MaxValue).ToList();

            return clientCase;
        }

        /// <summary>
        /// Retrieve the aries application information associated with a certain case.
        /// </summary>
        public async Task<IEnumerable<ApplicationModel>> GetAriesApplications(string clientId)
        {
            var response = await _esbClient.GetAsync<AriesAppResponse>($"aries/client/{clientId}/applications");

            if (!response.ARIESa7.optOPTION.Equals("00"))
            {
                return null;
            }

            var applications = response.ARIESa7.Applications;

            if (applications == null)
            {
                return null;
            }

            return applications.Where(a => a.Status != "Filed in Error").OrderByDescending(a => a.ReceivedDate);
        }

        /// <summary>
        /// Retrieve the aries cases associated with a certain client. Pulls the benefits
        /// associated with each returned case.
        /// </summary>
        public async Task<IEnumerable<CaseModel>> GetFormattedAriesCases(string clientId)
        {
            var response = await _esbClient.GetAsync<AriesCasesResponse>($"aries/client/{clientId}/case");

            if (response == null || !response.ARIESa3.optOPTION.Equals("00"))
            {
                return null;
            }

            var cases = response.ARIESa3.CaseList.ToList();

            //Client ID used isn't available in Aries service, so we are populating it here
            for (var i = 0; i < cases.Count; i++)
            {
                cases[i].ClientId = clientId;
            }

            return cases;
        }
    }
}