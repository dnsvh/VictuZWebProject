using Microsoft.AspNetCore.Mvc;

namespace VictuZWebProject.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
