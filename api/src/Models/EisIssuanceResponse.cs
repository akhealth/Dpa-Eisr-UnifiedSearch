using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Globalization;

namespace SearchApi.Models
{
    public partial class EisIssuanceResponse
    {
        public cHES18F05 cHES18F05 { get; set; }
    }

    public partial class cHES18F05
    {
        public string optDESCRIPTION { get; set; }
        public string optOPTION { get; set; }
        public string outBenefitMonth { get; set; }
        public string outBenefitType1 { get; set; }
        public string outBenefitType2 { get; set; }
        public string outBenefitType3 { get; set; }
        public string outBenefitType4 { get; set; }
        public string outBenefitType5 { get; set; }
        public string outCaseNumber { get; set; }
        public string outIssueType1 { get; set; }
        public string outIssueType2 { get; set; }
        public string outIssueType3 { get; set; }
        public string outIssueType4 { get; set; }
        public string outIssueType5 { get; set; }
        public string outIssuedAmount1 { get; set; }
        public string outIssuedAmount2 { get; set; }
        public string outIssuedAmount3 { get; set; }
        public string outIssuedAmount4 { get; set; }
        public string outIssuedAmount5 { get; set; }
        public string outIssuedDate1 { get; set; }
        public string outIssuedDate2 { get; set; }
        public string outIssuedDate3 { get; set; }
        public string outIssuedDate4 { get; set; }
        public string outIssuedDate5 { get; set; }
        public string outProgramType { get; set; }

        [JsonIgnore]
        public IEnumerable<string> BenefitTypes
        {
            get
            {
                return new List<string>(){
                    outBenefitType1,
                    outBenefitType2,
                    outBenefitType3,
                    outBenefitType4,
                    outBenefitType5
                }
                .Where(n => n != null);
            }
        }

        [JsonIgnore]
        public IEnumerable<string> IssuanceTypes
        {
            get
            {
                return new List<string>(){
                    outIssueType1,
                    outIssueType2,
                    outIssueType3,
                    outIssueType4,
                    outIssueType5
                }
                .Where(n => n != null);
            }
        }

        [JsonIgnore]
        public IEnumerable<string> IssuanceAmounts
        {
            get
            {
                return new List<string>(){
                    outIssuedAmount1,
                    outIssuedAmount2,
                    outIssuedAmount3,
                    outIssuedAmount4,
                    outIssuedAmount5
                }
                .Where(n => n != null);
            }
        }

        [JsonIgnore]
        public IEnumerable<string> IssuanceDates
        {
            get
            {
                return new List<string>(){
                    outIssuedDate1,
                    outIssuedDate2,
                    outIssuedDate3,
                    outIssuedDate4,
                    outIssuedDate5
                }
                .Where(n => n != null);
            }
        }
    }
}
