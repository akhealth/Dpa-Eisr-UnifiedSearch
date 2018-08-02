using System;
using SearchApi.Clients;

namespace SearchApi.Repositories
{
    public class BaseRepository
    {
        protected IEsbClient _esbClient { get; set; }

        public BaseRepository(IEsbClient esbClient)
        {
            _esbClient = esbClient;
        }

        /// <summary>
        /// Helper method to verify that a string from the ESB is valid, and doesn't contain the
        /// value {"@nil":"true"}
        /// </summary>
        protected static string VerifyResponseData(string responseData, string defaultData)
        {
            if (String.IsNullOrEmpty(responseData) || responseData.Equals("{\"@nil\":\"true\"}"))
            {
                return defaultData;
            }

            return responseData;
        }
    }
}