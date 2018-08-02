using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using System;
using System.Globalization;

namespace SearchApi.Models
{
    public partial class EisCasesResponse
    {
        public CHES18F03 CHES18F03 { get; set; }
    }

    public partial class CHES18F03
    {
        public string optDESCRIPTION { get; set; }
        public string optOPTION { get; set; }
        public string outAFBenMonth { get; set; }
        public string outAFBenStartDate { get; set; }
        public string outAFCaseNumber { get; set; }
        public string outAFPIFirst { get; set; }
        public string outAFPIInit { get; set; }
        public string outAFPILast { get; set; }
        public string outAFPIid { get; set; }
        public string outAFProgSubtype { get; set; }
        public string outAFProgramStatus { get; set; }
        public string outAFProgramStatusDesc { get; set; }
        public string outAFRecertMonthDue { get; set; }
        public string outAPBenMonth { get; set; }
        public string outAPBenStartDate { get; set; }
        public string outAPCaseNumber { get; set; }
        public string outAPPIFirst { get; set; }
        public string outAPPIInit { get; set; }
        public string outAPPILast { get; set; }
        public string outAPPIid { get; set; }
        public string outAPProgSubType { get; set; }
        public string outAPProgramStatus { get; set; }
        public string outAPProgramStatusDesc { get; set; }
        public string outAPRecertMonthDue { get; set; }
        public string outDuplicateClient { get; set; }
        public string outFSBenMonth { get; set; }
        public string outFSBenStartDate { get; set; }
        public string outFSCaseNumber { get; set; }
        public string outFSPIFirst { get; set; }
        public string outFSPIInit { get; set; }
        public string outFSPILast { get; set; }
        public string outFSPIid { get; set; }
        public string outFSProgSubtype { get; set; }
        public string outFSProgramStatus { get; set; }
        public string outFSProgramStatusDesc { get; set; }
        public string outFSRecertMonthDue { get; set; }
        public string outGABenMonth { get; set; }
        public string outGABenStartDate { get; set; }
        public string outGACaseNumber { get; set; }
        public string outGAPIFirst { get; set; }
        public string outGAPIInit { get; set; }
        public string outGAPILast { get; set; }
        public string outGAPIid { get; set; }
        public string outGAProgSubtype { get; set; }
        public string outGAProgramStatus { get; set; }
        public string outGAProgramStatusDesc { get; set; }
        public string outGARecertMonthDue { get; set; }
        public string outGMBenMonth { get; set; }
        public string outGMBenStartDate { get; set; }
        public string outGMCaseNumber { get; set; }
        public string outGMPIFirst { get; set; }
        public string outGMPIInit { get; set; }
        public string outGMPILast { get; set; }
        public string outGMPIid { get; set; }
        public string outGMProgSubtype { get; set; }
        public string outGMProgramStatus { get; set; }
        public string outGMProgramStatusDesc { get; set; }
        public string outGMRecertMonthDue { get; set; }
        public string outIABenMonth { get; set; }
        public string outIABenStartDate { get; set; }
        public string outIACaseNumber { get; set; }
        public string outIAPIFirst { get; set; }
        public string outIAPIInit { get; set; }
        public string outIAPILast { get; set; }
        public string outIAPIid { get; set; }
        public string outIAProgSubtype { get; set; }
        public string outIAProgramStatus { get; set; }
        public string outIAProgramStatusDesc { get; set; }
        public string outIARecertMonthDue { get; set; }
        public string outInputClientNum { get; set; }
        public string outMEBenMonth { get; set; }
        public string outMEEligEndDate { get; set; }
        public string outMEEligStartDate { get; set; }
        public string outMECaseNumber { get; set; }
        public string outMEPIFirst { get; set; }
        public string outMEPIInit { get; set; }
        public string outMEPILast { get; set; }
        public string outMEPIid { get; set; }
        public string outMEProgSubtype { get; set; }
        public string outMEProgramStatus { get; set; }
        public string outMEProgramStatusDesc { get; set; }
        public string outNewClientNum { get; set; }

        [JsonIgnore]
        public CaseModel AF
        {
            get
            {
                string certStart = GetCertificateDateString(outAFBenStartDate);
                string certEnd = GetCertificateDateString(outAFRecertMonthDue);
                return new CaseModel
                {
                    Location = "EIS",
                    CaseNumber = outAFCaseNumber,
                    ClientId = outNewClientNum,
                    PrimaryIndividual = new PrimaryIndividual
                    {
                        FirstName = outAFPIFirst,
                        LastName = outAFPILast,
                        ClientId = outAFPIid
                    },
                    Programs = new List<ProgramModel>
                    {
                        new ProgramModel
                        {
                            ProgramName = "AF",
                            ProgramSubtype = outAFProgSubtype,
                            ProgramStatus = outAFProgramStatusDesc,
                            BenefitsMonth = outAFBenMonth,
                            CertificationStart = certStart,
                            CertificationEnd = certEnd
                        }
                    }
                };
            }
        }

        [JsonIgnore]
        public CaseModel AP
        {
            get
            {
                string certStart = GetCertificateDateString(outAPBenStartDate);
                string certEnd = GetCertificateDateString(outAPRecertMonthDue);
                return new CaseModel
                {
                    Location = "EIS",
                    CaseNumber = outAPCaseNumber,
                    ClientId = outNewClientNum,
                    PrimaryIndividual = new PrimaryIndividual
                    {
                        FirstName = outAPPIFirst,
                        LastName = outAPPILast,
                        ClientId = outAPPIid
                    },
                    Programs = new List<ProgramModel>
                    {
                        new ProgramModel
                        {
                            ProgramName = "AP",
                            ProgramSubtype = outAPProgSubType,
                            ProgramStatus = outAPProgramStatusDesc,
                            BenefitsMonth = outAPBenMonth,
                            CertificationStart = certStart,
                            CertificationEnd = certEnd
                        }
                    }
                };
            }
        }

        [JsonIgnore]
        public CaseModel FS
        {
            get
            {
                string certStart = GetCertificateDateString(outFSBenStartDate);
                string certEnd = GetCertificateDateString(outFSRecertMonthDue);
                return new CaseModel
                {
                    Location = "EIS",
                    CaseNumber = outFSCaseNumber,
                    ClientId = outNewClientNum,
                    PrimaryIndividual = new PrimaryIndividual
                    {
                        FirstName = outFSPIFirst,
                        LastName = outFSPILast,
                        ClientId = outFSPIid
                    },
                    Programs = new List<ProgramModel>
                    {
                        new ProgramModel
                        {
                            ProgramName = "FS",
                            ProgramSubtype = outFSProgSubtype,
                            ProgramStatus = outFSProgramStatusDesc,
                            BenefitsMonth = outFSBenMonth,
                            CertificationStart = certStart,
                            CertificationEnd = certEnd
                        }
                    }
                };
            }
        }

        public CaseModel GA
        {
            get
            {
                string certStart = GetCertificateDateString(outGABenStartDate);
                string certEnd = GetCertificateDateString(outGARecertMonthDue);
                return new CaseModel
                {
                    Location = "EIS",
                    CaseNumber = outGACaseNumber,
                    ClientId = outNewClientNum,
                    PrimaryIndividual = new PrimaryIndividual
                    {
                        FirstName = outGAPIFirst,
                        LastName = outGAPILast,
                        ClientId = outGAPIid
                    },
                    Programs = new List<ProgramModel>
                    {
                        new ProgramModel
                        {
                            ProgramName = "GA",
                            ProgramSubtype = outGAProgSubtype,
                            ProgramStatus = outGAProgramStatusDesc,
                            BenefitsMonth = outGABenMonth,
                            CertificationStart = certStart,
                            CertificationEnd = certEnd
                        }
                    }
                };
            }
        }

        public CaseModel GM
        {
            get
            {
                string certStart = GetCertificateDateString(outGMBenStartDate);
                string certEnd = GetCertificateDateString(outGMRecertMonthDue);
                return new CaseModel
                {
                    Location = "EIS",
                    CaseNumber = outGMCaseNumber,
                    ClientId = outNewClientNum,
                    PrimaryIndividual = new PrimaryIndividual
                    {
                        FirstName = outGMPIFirst,
                        LastName = outGMPILast,
                        ClientId = outGMPIid
                    },
                    Programs = new List<ProgramModel>
                    {
                        new ProgramModel
                        {
                            ProgramName = "GM",
                            ProgramSubtype = outGMProgSubtype,
                            ProgramStatus = outGMProgramStatusDesc,
                            BenefitsMonth = outGMBenMonth,
                            CertificationStart = certStart,
                            CertificationEnd = certEnd
                        }
                    }
                };
            }
        }

        public CaseModel IA
        {
            get
            {
                string certStart = GetCertificateDateString(outIABenStartDate);
                string certEnd = GetCertificateDateString(outIARecertMonthDue);
                return new CaseModel
                {
                    Location = "EIS",
                    CaseNumber = outIACaseNumber,
                    ClientId = outNewClientNum,
                    PrimaryIndividual = new PrimaryIndividual
                    {
                        FirstName = outIAPIFirst,
                        LastName = outIAPILast,
                        ClientId = outIAPIid
                    },
                    Programs = new List<ProgramModel>
                    {
                        new ProgramModel
                        {
                            ProgramName = "IA",
                            ProgramSubtype = outIAProgSubtype,
                            ProgramStatus = outIAProgramStatusDesc,
                            BenefitsMonth = outIABenMonth,
                            CertificationStart = certStart,
                            CertificationEnd = certEnd
                        }
                    }
                };
            }
        }

        public CaseModel ME
        {
            get
            {
                string certStart = GetCertificateDateString(outMEEligStartDate);
                string certEnd = GetCertificateDateString(outMEEligEndDate);
                return new CaseModel
                {
                    Location = "EIS",
                    CaseNumber = outMECaseNumber,
                    ClientId = outNewClientNum,
                    PrimaryIndividual = new PrimaryIndividual
                    {
                        FirstName = outMEPIFirst,
                        LastName = outMEPILast,
                        ClientId = outMEPIid
                    },
                    Programs = new List<ProgramModel>
                    {
                        new ProgramModel
                        {
                            ProgramName = "ME",
                            ProgramSubtype = outMEProgSubtype,
                            ProgramStatus = outMEProgramStatusDesc,
                            BenefitsMonth = outMEBenMonth,
                            CertificationStart = certStart,
                            CertificationEnd = certEnd
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
                    AF,
                    AP,
                    FS,
                    GA,
                    GM,
                    IA,
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

        private string GetCertificateDateString(string dateString)
        {
            if (dateString == null || dateString == "0" || dateString == "")
            {
                return "";
            }
            else
            {
                DateTime outDate;
                string parseFormat = dateString.Length == 6 ? "yyyyMM" : "yyyyMMdd";
                DateTime.TryParseExact(dateString, parseFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out outDate);
                return outDate.ToShortDateString();
            }
        }
    }
}