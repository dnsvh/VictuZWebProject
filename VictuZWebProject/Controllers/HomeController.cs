using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using VictuZWebProject.Models;

namespace VictuZWebProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public ActionResult IndexActivitiesFromHome()
        {
            return RedirectToAction("Index", "Activities_memberview_");
        }
        public ActionResult UpcomingActivitiesFromHome()
        {
            return RedirectToAction("UpcomingActivities", "Activities_memberview_");
        }

        public IActionResult Index()
        {
            return View();
        }


        // need to be loggein to access this page
        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
