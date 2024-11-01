using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZ_Lars.Data;
using VictuZ_Lars.Models;
using VictuZWebProject.Models;

namespace VictuZ_Lars.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly VictuZ_Lars_Db _context;

        public ActivitiesController(VictuZ_Lars_Db context)
        {
            _context = context;
        }

        // GET: Activities
        public async Task<IActionResult> Index()
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

        // GET: Activities/Details/5
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

        //public async Task<IActionResult> Register(int? Id)
        //{
        //    if (Id == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        //
        //    var existingRegistration = await _context.UserRegistration
        //        .FirstOrDefaultAsync(r => r.UserId == userId && r.ActivityId == Id);
        //
        //    if (existingRegistration != null)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //
        //    var registration = new UserRegistration
        //    {
        //        UserId = userId,
        //        ActivityId = (int)Id,
        //    };
        //
        //
        //    _context.UserRegistration.Add(registration);
        //
        //    var activity = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
        //    if (activity == null)
        //    {
        //        return NotFound();
        //    }
        //
        //    if (activity.Registered < activity.MaxCapacity)
        //    {
        //        // Update the database
        //        activity.Registered += 1;
        //        _context.Activity.Update(activity);
        //        _context.UserRegistration.Add(registration);
        //        await _context.SaveChangesAsync();
        //
        //        return RedirectToAction("Index");
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}

        // GET: Activities/Create
        [Authorize]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("ActivityId,Name,Body,Host,Location,ImageUrl,Registered,MaxCapacity,DatePublished,DateDue")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        // GET: Activities/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activity.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            return View(activity);
        }

        // POST: Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityId,Name,Body,Host,Location,ImageUrl,Registered,MaxCapacity,DatePublished,DateDue")] Activity activity)
        {
            if (id != activity.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.ActivityId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(activity);
        }

        // GET: Activities/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Activities/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activity.FindAsync(id);
            if (activity != null)
            {
                _context.Activity.Remove(activity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(int id)
        {
            return _context.Activity.Any(e => e.ActivityId == id);
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

            // Get the current user ID
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Check if liked
            var existingRegister = await _context.UserRegistration
                .FirstOrDefaultAsync(l => l.UserId == userId && l.ActivityId == Id);

            var registration = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == Id);
            if (registration == null)
            {
                return NotFound();
            }

            if (existingRegister != null)
            {
                // delete registration
                _context.UserRegistration.Remove(existingRegister);
                registration.Registered -= 1;
            }
            else
            {
                // register
                var reg = new UserRegistration
                {
                    UserId = userId,
                    ActivityId = (int)Id
                };

                _context.UserRegistration.Add(reg);
                registration.Registered += 1; 
            }

            // Update
            _context.Activity.Update(registration);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index", "Activities");
        }

    }
}
