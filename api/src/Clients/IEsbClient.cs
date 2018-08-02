using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace SearchApi.Clients
{
    /// <summary>
    /// Abstraction of the connection to the ESB.
    /// </summary>
    public interface IEsbClient
    {
        /// <summary>
        /// Implements a HttpGet function to the esb.
        /// </summary>
        ObjectResult Get(string uri);

        Task<ObjectResult> GetAsync(string uri);

        Task<T> GetAsync<T>(string uri);

        /// <summary>
        /// Implements a HttpPost function to the esb.
        /// </summary>
        ObjectResult Post(string uri, object body);

        Task<ObjectResult> PostAsync(string uri, object body);

        Task<T> PostAsync<T>(string uri, object body);
    }
}