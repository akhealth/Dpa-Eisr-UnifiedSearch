using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SearchApi.Clients;
using SearchApi.Controllers;
using SearchApi.Models;
using SearchApi.Repositories;

namespace SearchApi.Controllers
{
    /// <summary>
    /// Controller to retrieve information on the status of the API, including description of OS,
    /// process information, presence of ESB connection, and request headers.
    /// </summary>
    [Route("status")]
    public class StatusController : BaseController
    {

        private IConfiguration _configuration { get; set; }
        private IHostingEnvironment _environment { get; set; }

        private IMciRepository _mciRepository { get; set; }

        public StatusController(
            IConfiguration configuration,
            IHostingEnvironment environment,
            IMciRepository mciRepository,
            ILogger<StatusController> logger) : base(logger)
        {
            _configuration = configuration;
            _environment = environment;
            _mciRepository = mciRepository;
        }

        /// <summary>
        /// Get information about the current status of the Search API.
        /// </summary>
        /// <remarks>
        /// In production, doesn't return any data.
        /// </remarks>
        /// <response code="200">
        /// { success = true, data = [ Host = [ OSDescription, FrameworkDescription ], Process = [ Id, ProcessName, StartTime ], EsbConnection, Configuration, Request ] }
        /// </response>
        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            if (_environment.IsProduction())
            {
                return Ok();
            }

            var esbResult = await _mciRepository.GetMci(null, null, null);

            var process = Process.GetCurrentProcess();

            return Ok(new
            {
                Host = new
                {
                    OSDescription = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
                    FrameworkDescription = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription,
                },
                Process = new
                {
                    Id = process.Id,
                    ProcessName = process.ProcessName,
                    StartTime = process.StartTime
                },
                EsbConnection = esbResult != null,
                Configuration = _configuration.AsEnumerable(),
                Request = (IEnumerable)Request?.Headers
            });
        }
    }
}