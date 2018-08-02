using System.Collections.Generic;

namespace SearchApi.Models
{
    public class CaseModel
    {
        public string Location { get; set; }
        public string CaseNumber { get; set; }
        public string ClientId { get; set; }
        public List<ProgramModel> Programs { get; set; }
        public PrimaryIndividual PrimaryIndividual { get; set; }
    }

    public class PrimaryIndividual
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ClientId { get; set; }
    }
}