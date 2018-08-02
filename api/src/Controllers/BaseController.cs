using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SearchApi.Utilities;

namespace SearchApi.Controllers
{
    /// <summary>
    /// A base class for new controllers, overrides the ok(data) and BadRequest(data) methods.
    /// </summary>
    /// <remarks>
    /// All new controllers returning results outside of the API should inherit from this
    /// class and return those results using these methods.
    /// </remarks>
    public class BaseController : Controller
    {
        protected ILogger _logger { get; set; }

        public BaseController(ILogger logger)
        {
            _logger = logger;
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new OkObjectResult Ok()
        {
            return new OkObjectResult(JsonResponse.ok());
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override OkObjectResult Ok(object value)
        {
            return new OkObjectResult(JsonResponse.ok(value));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public new BadRequestObjectResult BadRequest()
        {
            return new BadRequestObjectResult(JsonResponse.error("Bad Request"));
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public override BadRequestObjectResult BadRequest(object value)
        {
            return new BadRequestObjectResult(JsonResponse.error(value));
        }
    }
}