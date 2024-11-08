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

        public async Task<IActionResult> Register(int? Id, string returnUrl = null)
        {

            if (Id == null)
            {
                return NotFound();
            }


            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            bool isMember = User.IsInRole("Member") || User.IsInRole("Staff") || User.IsInRole("Admin");
            bool isVisitor = User.IsInRole("Visitor");


            var activity = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
            if (activity == null)
            {
                return NotFound();
            }

            var existingRegister = await _context.UserRegistration
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ActivityId == Id);

            var registration = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
            if (registration == null)
            {
                return NotFound();
            }

            if (existingRegister != null)
            {

                _context.UserRegistration.Remove(existingRegister);
                activity.Registered -= 1;
            }
            else
            {

                int memberCount = _context.UserRegistration.Count(r => r.ActivityId == Id &&
                                  (User.IsInRole("Member") || User.IsInRole("Staff") || User.IsInRole("Admin")));
                int visitorCount = _context.UserRegistration.Count(r => r.ActivityId == Id && User.IsInRole("Visitor"));

                if (isVisitor)
                {

                    if (visitorCount >= activity.MaxCapacity - activity.MembersOnlyCapacity)
                    {
                        return BadRequest("Geen beschikbare plaatsen voor bezoekers.");
                    }
                }
                else if (isMember)
                {
                    // Leden kunnen zich registreren zolang MaxCapacity niet bereikt is
                    if (activity.Registered >= activity.MaxCapacity)
                    {
                        return BadRequest("De activiteit zit vol.");
                    }
                }

                // Voeg registratie toe
                var reg = new UserRegistration
                {
                    UserId = userId,
                    ActivityId = (int)Id
                };

                _context.UserRegistration.Add(reg);
                activity.Registered += 1;
            }


            _context.Activity.Update(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> Unregister(int? id, string returnUrl = null)
        {
            if (id == null)
            {
                return NotFound();
            }


            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            var activity = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null)
            {
                return NotFound();
            }


            var existingRegister = await _context.UserRegistration
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ActivityId == id);

            if (existingRegister != null)
            {

                _context.UserRegistration.Remove(existingRegister);
                activity.Registered -= 1;


                await _context.SaveChangesAsync();
            }
            else
            {

                return NotFound();
            }


            return RedirectToAction(nameof(Index));
        }


    }
}
