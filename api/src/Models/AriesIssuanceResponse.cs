using System;
using System.Globalization;
using Newtonsoft.Json;

namespace SearchApi.Models
{
    public partial class AriesIssuanceResponse
    {
        public ARIESa6 ARIESa6 { get; set; }
    }

    public partial class ARIESa6
    {
        public string optDESCRIPTION { get; set; }
        public string optOPTION { get; set; }
        public string outBenefitMonth { get; set; }
        public string outCaseNum { get; set; }
        public string outIssuance_dt { get; set; }
    }
}
