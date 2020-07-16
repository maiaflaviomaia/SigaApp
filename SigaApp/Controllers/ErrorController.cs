using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SigaApp.Controllers
{
    [Authorize]
    public class ErrorController : Controller
    {
        [HttpGet("Error/{httpStatusCode}")]
        public IActionResult Index(int httpStatusCode)
        {
            return View(httpStatusCode);
        }
    }
}