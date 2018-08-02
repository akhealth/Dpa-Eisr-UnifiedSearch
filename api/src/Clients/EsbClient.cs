using System.IO;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RestSharp;
using Newtonsoft.Json;
using System;
using RestSharp.Authenticators;
using System.Runtime.InteropServices;
using System.Net;

namespace SearchApi.Clients
{
    /// <summary>
    /// An implementation of the IEsbClient interface using RestSharp.
    /// </summary>
    public class EsbClient : Controller, IEsbClient
    {
        private IConfiguration _configuration { get; set; }

        public RestClient _client;

        // This is the request timeout in milliseconds
        private int requestTimeout = 100 * 1000;

        private string _esbTestUrl
        {
            get { return _configuration.GetSection("ESB_URL").Value; }
        }

        private string _esbAuthUser
        {
            get { return _configuration.GetSection("ESB_AUTH_USER").Value; }
        }

        private string _esbAuthPass
        {
            get { return _configuration.GetSection("ESB_AUTH_PASS").Value; }
        }

        public EsbClient(IConfiguration configuration)
        {
            this._configuration = configuration;

            this._client = new RestClient(_esbTestUrl)
            {
                Authenticator = new HttpBasicAuthenticator(_esbAuthUser, _esbAuthPass)
            };
        }

        public ObjectResult Get(string uri)
        {
            var request = new RestRequest(uri)
            {
                Method = Method.GET,
                Timeout = requestTimeout
            };

            var response = _client.Execute<object>(request);

            CheckResponse(response);

            return Ok(response.Content);
        }

        public async Task<ObjectResult> GetAsync(string uri)
        {
            var request = new RestRequest(uri)
            {
                Method = Method.GET,
                Timeout = requestTimeout
            };

            var response = await _client.ExecuteTaskAsync<object>(request);

            CheckResponse(response);

            return Ok(response.Data);
        }

        public async Task<T> GetAsync<T>(string uri)
        {
            var request = new RestRequest(uri)
            {
                Method = Method.GET,
                Timeout = requestTimeout
            };

            var response = await _client.ExecuteTaskAsync<T>(request);

            CheckResponse(response);

            return response.Data;
        }

        public ObjectResult Post(string uri, object body)
        {
            var request = new RestRequest(uri)
            {
                Method = Method.POST,
                Timeout = requestTimeout
            }
            .AddJsonBody(body);

            var response = _client.Execute<object>(request);

            CheckResponse(response);

            return Ok(response.Data);
        }

        public async Task<ObjectResult> PostAsync(string uri, object body)
        {
            var request = new RestRequest(uri)
            {
                Method = Method.POST,
                Timeout = requestTimeout
            }
            .AddJsonBody(body);

            var response = await _client.ExecuteTaskAsync<object>(request);

            CheckResponse(response);

            return Ok(response.Data);
        }

        public async Task<T> PostAsync<T>(string uri, object body)
        {
            var request = new RestRequest(uri)
            {
                Method = Method.POST,
                Timeout = requestTimeout
            }
            .AddJsonBody(body);

            var response = await _client.ExecuteTaskAsync<T>(request);

            CheckResponse(response);

            return response.Data;
        }

        private void CheckResponse(IRestResponse response)
        {
            CheckResponseStatus(response);
            CheckTimeout(response);
        }

        private void CheckTimeout(IRestResponse response)
        {
            if (response?.ResponseStatus == ResponseStatus.TimedOut)
            {
                throw new TimeoutException();
            }
        }

        private void CheckResponseStatus(IRestResponse response)
        {
            if (response == null || response.ResponseStatus == ResponseStatus.Error)
            {
                throw new ExternalException(
                    "An error occurred while making request to ESB",
                    response?.ErrorException ?? null
                );
            }

            if (response.StatusCode == HttpStatusCode.Forbidden || response.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new ExternalException(
                    "Request to ESB was forbidden/unauthorized. You may need to check the application's environment variables.",
                    response?.ErrorException ?? null
                );
            }
        }
    }
}
