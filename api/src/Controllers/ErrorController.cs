using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace SearchApi.Controllers
{
    /// <summary>
    /// Default ErrorController - any uncaught exceptions are passed through this
    /// controller and, depending on the environment, a json response is returned to the client
    /// with the appropriate level of information.
    /// </summary>
    [Route("error")]
    public class ErrorController : BaseController
    {
        private IHostingEnvironment _env;
        private IConfiguration _configuration { get; set; }

        public ErrorController(IConfiguration configuration, IHostingEnvironment env, ILogger<SearchController> logger) : base(logger)
        {
            _configuration = configuration;
            _env = env;
        }

        /// <summary>
        /// Handles uncaught exceptions, and returns a json response with pertinent information.
        /// </summary>
        /// <remarks>
        /// If Env=Development then:
        ///   errmsg = { error message, query, stack, headers }
        /// Else:
        ///   errmsg = { error message, query }
        /// </remarks>
        /// <response code="400">
        /// { success = false, data = errmsg }
        /// </response>
        [HttpGet]
        [ProducesResponseType(400)]
        public IActionResult Error()
        {
            var excHandler = HttpContext.Features.Get<IExceptionHandlerFeature>();
            object errMsg;
            if (_env.IsDevelopment())
            {
                errMsg = new
                {
                    error = excHandler.Error.Message,
                    query = HttpContext.Request.Query,
                    stack = excHandler.Error.StackTrace.Split("\n"),
                    headers = HttpContext.Request.Headers,
                    configuration = _configuration
                };
            }
            else
            {
                errMsg = new
                {
                    error = excHandler.Error.Message,
                    query = HttpContext.Request.Query
                };
            }
            return BadRequest(errMsg);
        }
    }
}
