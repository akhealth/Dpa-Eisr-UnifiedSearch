using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;
using RestSharp.Deserializers;
using System;

namespace SearchApi.Models
{
    public partial class AriesAppResponse
    {
        public ARIESa7 ARIESa7 { get; set; }
    }

    public partial class ARIESa7
    {
        public string optDESCRIPTION { get; set; }
        public string optOPTION { get; set; }
        public ApplicationList outApplication { get; set; }
        public string outIndvID { get; set; }

        [JsonIgnore]
        public List<ApplicationModel> Applications
        {
            get
            {
                if (outApplication.AriesApplication == null)
                {
                    return null;
                }

                var applicationList = new List<ApplicationModel>();
                foreach (var appInfo in outApplication.AriesApplication)
                {
                    DateTime outDate;
                    DateTime.TryParse(appInfo.AppRecvdDate, out outDate);
                    applicationList.Add(new ApplicationModel()
                    {
                        ApplicationNumber = appInfo.AppNumber,
                        Status = appInfo.AppStatusDesc,
                        ReceivedDate = outDate,
                        Source = appInfo.AppStatusModeDesc
                    });
                }
                return applicationList;
            }
        }
    }

    public partial class ApplicationList
    {
        [DeserializeAs(Name = "ARIESa7.AriesApplication")]
        public List<ApplicationInformation> AriesApplication { get; set; }
    }

    public partial class ApplicationInformation
    {
        public string AppNumber { get; set; }
        public string AppRecvdDate { get; set; }
        public string AppStatusCode { get; set; }
        public string AppStatusDesc { get; set; }
        public string AppStatusMode { get; set; }
        public string AppStatusModeDesc { get; set; }
    }
}