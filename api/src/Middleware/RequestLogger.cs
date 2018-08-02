using System.Collections;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Serilog.Context;

namespace SearchApi.Middleware
{
    /// <summary>
    /// A middleware for enriching logs with custom search app data
    /// </summary>
    public class RequestLogger
    {
        private readonly RequestDelegate _next;

        public RequestLogger(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Add available information about the origin of the request
            LogContext.PushProperty("RequestOrigin", context.Connection.RemoteIpAddress);
            LogContext.PushProperty("RequestHeaders", (IEnumerable)context.Request?.Headers);

            // Add ARIES user information
            var user = context.Request.Headers["iv-user"].FirstOrDefault();
            if (user != null)
            {
                LogContext.PushProperty("User", user);
            }

            // Add additional process information
            var process = Process.GetCurrentProcess();
            LogContext.PushProperty("ProcessId", process.Id);
            LogContext.PushProperty("ProcessName", process.ProcessName);
            LogContext.PushProperty("StartTime", process.StartTime);

            await _next.Invoke(context);
        }
    }
}