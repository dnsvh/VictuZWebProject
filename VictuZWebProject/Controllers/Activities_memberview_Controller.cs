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
