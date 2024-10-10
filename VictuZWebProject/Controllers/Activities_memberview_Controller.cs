using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZ_Lars.Data;
using VictuZ_Lars.Models;
using VictuZWebProject.Models;
using VictuZWebProject.Data;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;
using Activity = VictuZ_Lars.Models.Activity;

namespace VictuZ_Lars.Controllers
{
    public class Activities_memberview_Controller : Controller
    {
        private readonly VictuZ_Lars_Db _context;

        public Activities_memberview_Controller(VictuZ_Lars_Db context)
        {
            _context = context;
        }

        // GET: Activities_memberview_
        public async Task<IActionResult> Index()
        {
            var activities = await _context.Activity.ToListAsync();
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

        public async Task<IActionResult> UpcomingActivities(DateTime? filterDate = null)
        {
            // Get the current date and time
            DateTime currentDateTime = DateTime.Now;

            // Query all activities from the database
            IQueryable<Activity> activities = _context.Activity;

            // If a filter date is provided, filter activities by the given date and time
            if (filterDate.HasValue)
            {
                activities = activities.Where(a => a.DateDue >= filterDate.Value);
            }
            else
            {
                // Otherwise, filter activities with a future date and time compared to the current time
                activities = activities.Where(a => a.DateDue > currentDateTime);
            }

            // Order by date and time so the earliest upcoming activity is first
            var upcomingActivities = await activities
                .OrderBy(a => a.DateDue)
                .ToListAsync();

            // Pass the filtered activities to the view
            return View(upcomingActivities);
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

        public async Task<IActionResult> Register(int? Id)
        {
            if (Id == null)
            {
                return NotFound();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var existingRegistration = await _context.UserRegistration
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ActivityId == Id);

            if (existingRegistration != null)
            {
                return RedirectToAction("Index"); 
            }

            var registration = new UserRegistration
            {
                UserId = userId,
                ActivityId = (int)Id,
            };


            _context.UserRegistration.Add(registration);

            var activity = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
            if (activity == null)
            {
                return NotFound();
            }

            if (activity.Registered < activity.MaxCapacity)
            {
                // Update the database
                activity.Registered += 1;
                _context.Activity.Update(activity);
                _context.UserRegistration.Add(registration);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete(int Id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Find the registration record to delete
            var registration = await _context.UserRegistration
                .FirstOrDefaultAsync(r => r.UserId == userId && r.ActivityId == Id);

            var activity = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
            if (registration != null)
            {
                activity.Registered -= 1;
                _context.Activity.Update(activity);

                _context.UserRegistration.Remove(registration);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index"); // Redirect back to the index after deletion
        }
    }
}
