using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZWebProject.Data;
using VictuZ_Lars.Models;
using VictuZWebProject.Models;
using VictuZ_Lars.Data;

namespace VictuZWebProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly VictuZ_Lars_Db _context;

        public HomeController(VictuZ_Lars_Db context)
        {
            _context = context;
        }

        public ActionResult IndexActivitiesFromHome()
        {
            return RedirectToAction("Index", "Activities_memberview_");
        }
        public ActionResult UpcomingActivitiesFromHome()
        {
            return RedirectToAction("UpcomingActivities", "Activities_memberview_");
        }

        public async Task<IActionResult> IndexAsync()
        {
            var activities = await _context.Activity
            .OrderBy(a => a.DateDue) // Voeg deze regel toe om te sorteren
            .ToListAsync();
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var userRegistrations = await _context.UserRegistration
                .Where(r => r.UserId == userId)
                .Select(r => r.ActivityId)
                .ToListAsync();

            var viewModel = activities.Select(a => new ActivityViewModel
            {
                Activity = a,
                IsUserRegistered = userRegistrations.Contains(a.ActivityId)
            }).ToList();

            return View(viewModel);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
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
            return View(new ErrorViewModel { RequestId = DateTime.UtcNow.Ticks.ToString() });
        }

    }
}
