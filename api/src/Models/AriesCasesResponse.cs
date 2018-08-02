using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace SearchApi.Models
{
    public partial class AriesCasesResponse
    {
        public ARIESa3 ARIESa3 { get; set; }
    }

    public partial class ARIESa3
    {
        public string optDESCRIPTION { get; set; }
        public string optOPTION { get; set; }
        public string outCaseNumber { get; set; }
        public string outCaseStatus { get; set; }
        public string outCaseStatusDescription { get; set; }
        public string outHeadOfHouseholdFName { get; set; }
        public string outHeadOfHouseholdID { get; set; }
        public string outHeadOfHouseholdLName { get; set; }
        public string outHeadOfHouseholdMName { get; set; }
        public string outProgramType { get; set; }

        [JsonIgnore]
        public CaseModel ME
        {
            get
            {
                return new CaseModel
                {
                    Location = "ARIES",
                    CaseNumber = outCaseNumber,
                    PrimaryIndividual = new PrimaryIndividual
                    {
                        FirstName = outHeadOfHouseholdFName,
                        LastName = outHeadOfHouseholdLName,
                        ClientId = outHeadOfHouseholdID
                    },
                    Programs = new List<ProgramModel>
                    {
                        new ProgramModel
                        {
                            ProgramName = outProgramType,
                            ProgramStatus = outCaseStatusDescription
                        }
                    }
                };
            }
        }

        public IEnumerable<CaseModel> CaseList
        {
            get
            {
                var caseList = new List<CaseModel>(){
                    ME
                }
                .Where(c => c.CaseNumber != null);

                // This merges cases with the same case number together, combining their programs
                caseList = caseList.GroupBy(c => c.CaseNumber).Select(group => new CaseModel
                {
                    Location = group.First().Location,
                    CaseNumber = group.First().CaseNumber,
                    ClientId = group.First().ClientId,
                    PrimaryIndividual = group.First().PrimaryIndividual,
                    Programs = group.SelectMany(c => c.Programs).ToList()
                });

                return caseList;
            }
        }
    }
}
