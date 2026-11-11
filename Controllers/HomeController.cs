using Microsoft.AspNetCore.Mvc;

namespace Alumni.Controllers;

public class HomeController : Controller
{
    [HttpGet("/Home")]
    [HttpGet("/Home/Index")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/Home/About")]
    public IActionResult About()
    {
        return View();
    }

    [Route("/Home/NotFound")]
    [IgnoreAntiforgeryToken]
    [ApiExplorerSettings(IgnoreApi = true)]
    public IActionResult PageNotFound()
    {
        Response.StatusCode = StatusCodes.Status404NotFound;
        return View("NotFound");
    }
}
