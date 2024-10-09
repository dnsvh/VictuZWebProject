using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZ_Lars.Data;
using VictuZ_Lars.Models;

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
            return View(await _context.Activity.ToListAsync());
        }

        // GET: Activities_memberview_/Details/5
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

        public async Task<IActionResult> Register(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the activity by ID
            var activity = await _context.Activity.FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null)
            {
                return NotFound();
            }

            if (activity.Registered < activity.MaxCapacity)
            {
                // Update the database
                activity.Registered += 1;
                _context.Activity.Update(activity);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Index");
            }

            // Increment the registered count
        }
    }
}
