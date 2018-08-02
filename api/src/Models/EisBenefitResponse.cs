using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace SearchApi.Models
{
    public partial class EisBenefitResponse
    {
        public CHES18F04 CHES18F04 { get; set; }
    }

    public partial class CHES18F04
    {
        public string optDESCRIPTION { get; set; }
        public string optOPTION { get; set; }
        public string outBenefitMonth { get; set; }
        public string outEligibilityCode { get; set; }
        public string outIssuanceInd { get; set; }
        public string outMedSubtype { get; set; }
    }
}