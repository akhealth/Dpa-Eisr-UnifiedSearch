using System;

namespace SearchApi.Models
{
    public class ApplicationModel
    {
        public string ApplicationNumber { get; set; }
        public string Status { get; set; }
        public DateTime ReceivedDate { get; set; }
        public string Source { get; set; }
        public string ReceivedDateString
        {
            get
            {
                return ReceivedDate.ToShortDateString();
            }
        }
    }
}