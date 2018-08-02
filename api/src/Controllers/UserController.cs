using System.Collections;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using SearchApi.Clients;
using SearchApi.Controllers;
using SearchApi.Models;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace SearchApi.Controllers
{
    /// <summary>
    /// UserController returns information on the current user, such as username and group,
    /// from the request headers.
    /// </summary>
    [Route("user")]
    public class UserController : BaseController
    {
        private IHostingEnvironment _env { get; set; }

        public UserController(IHostingEnvironment env, ILogger<UserController> logger) : base(logger)
        {
            _env = env;
        }

        /// <summary>
        /// Get information about the current user.
        /// </summary>
        /// <remarks>
        /// In development environment, responds only with user - "developer" if user is null.
        /// Otherwise, gives full information about current user.
        /// </remarks>
        /// <response code="200">
        /// { success = true, data = [ user, groups, userdn ] }
        /// </response>
        [HttpGet]
        public IActionResult GetUserInfo()
        {
            var user = Request.Headers["iv-user"].FirstOrDefault();

            if (_env.IsDevelopment() && user == null)
            {
                return Ok(new
                {
                    User = "Developer"
                });
            }

            return Ok(new
            {
                User = user,
                Groups = Request.Headers["iv-groups"].FirstOrDefault(),
                UserDN = Request.Headers["iv-user-l"].FirstOrDefault()
            });
        }

        /// <summary>
        /// Stub endpoint for keeping the webseal session alive.
        /// </summary>
        /// <remarks>
        /// Returns no data, simply used to connect the web client to the API, keeping the webseal session active.
        /// </remarks>
        [HttpGet("refresh")]
        public IActionResult RefreshSession()
        {
            return Ok();
        }

        /// <summary>
        /// Create a log entry tracking the end of the user's session.
        /// API clients can call this method to record a log-out event.
        /// </summary>
        [HttpGet("end-session")]
        public IActionResult EndSession()
        {
            return Ok();
        }
    }
}
