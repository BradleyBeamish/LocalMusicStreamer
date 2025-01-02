using Microsoft.AspNetCore.Mvc;

namespace LocalMusicStreamer.Controllers;

public class HomeController : Controller
{
    /*
     * GET /Home
     */
    public IActionResult Index()
    {
        return View();
    }
}
