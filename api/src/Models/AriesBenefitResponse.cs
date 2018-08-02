namespace SearchApi.Models
{
    public partial class AriesBenefitResponse
    {
        public ARIESa4 ARIESa4 { get; set; }
    }

    public partial class ARIESa4
    {
        public string optDESCRIPTION { get; set; }
        public string optOPTION { get; set; }
        public string outBenMonth { get; set; }
        public string outClientID { get; set; }
        public string outEligBeginDate { get; set; }
        public string outEligEndDate { get; set; }
        public string outMedEligCode { get; set; }
        public string outMedIssueTypeDesc { get; set; }
        public string outMedSubType { get; set; }
    }
}
