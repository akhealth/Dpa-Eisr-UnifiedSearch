using Microsoft.AspNetCore.Mvc;

namespace SearchWeb.Controllers
{
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return File("~/index.html", "text/html");
        }

        public IActionResult Error(int? statusCode = null)
        {
            if (statusCode.HasValue)
            {
                if (statusCode == 404) // This can check for other status codes if we want the user to see any more
                {
                    var viewName = statusCode.ToString();
                    return File("~/" + viewName + ".html", "text/html");
                }
            }
            return File("~/error.html", "text/html");
        }
    }
}