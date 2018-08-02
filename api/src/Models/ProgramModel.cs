using System;
using System.Collections.Generic;

namespace SearchApi.Models
{
    public class ProgramModel
    {
        public string ProgramName { get; set; }
        public string ProgramSubtype { get; set; }
        public string ProgramStatus { get; set; }
        public string BenefitsMonth { get; set; }
        public string EligibilityCode { get; set; }
        public string MedicaidSubType { get; set; }
        public string CertificationStart { get; set; }
        public string CertificationEnd { get; set; }
        public List<IssuanceModel> Issuances { get; set; }

        public ProgramModel()
        {
            Issuances = new List<IssuanceModel>();
        }
    }

    public class IssuanceModel
    {
        public string BenefitType { get; set; }
        public string IssuanceType { get; set; }
        public string IssuanceAmount { get; set; }
        public DateTime IssuanceDate { get; set; }
        public string IssuanceDateString
        {
            get
            {
                return IssuanceDate.ToShortDateString();
            }
        }
    }
}
