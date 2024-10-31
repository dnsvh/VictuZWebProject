using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VictuZWebProject.Models;
using VictuZ_Lars.Data;
using Microsoft.AspNetCore.Identity;
using VictuZWebProject.Areas.Identity.Data;

namespace VictuZWebProject.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly VictuZ_Lars_Db _context;
        private readonly UserManager<AppUser> _userManager;

        public MembershipsController(VictuZ_Lars_Db context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Memberships
        public async Task<IActionResult> Index()
        {
            return View(await _context.Memberships.ToListAsync());
        }

        // GET: Memberships/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberships = await _context.Memberships
                .FirstOrDefaultAsync(m => m.Id == id);
            if (memberships == null)
            {
                return NotFound();
            }

            return View(memberships);
        }

        // GET: Memberships/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Memberships/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,UserId,MembershipId")] Memberships memberships)
        {
            if (ModelState.IsValid)
            {
                _context.Add(memberships);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(memberships);
        }

        // Get payment plan and confirm plan
        public IActionResult ConfirmFreePlan()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ConfirmFreePlanResponse(bool confirm)
        {
            if (confirm)
            {
                return RedirectToAction("Payment", new { plan = "Basic" });
            }
            return RedirectToAction("Index");
        }

        // GET: Memberships/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberships = await _context.Memberships.FindAsync(id);
            if (memberships == null)
            {
                return NotFound();
            }
            return View(memberships);
        }

        // POST: Memberships/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,UserId,MembershipId")] Memberships memberships)
        {
            if (id != memberships.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(memberships);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MembershipsExists(memberships.Id))
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
            return View(memberships);
        }

        // GET: Memberships/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var memberships = await _context.Memberships
                .FirstOrDefaultAsync(m => m.Id == id);
            if (memberships == null)
            {
                return NotFound();
            }

            return View(memberships);
        }

        // POST: Memberships/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var memberships = await _context.Memberships.FindAsync(id);
            if (memberships != null)
            {
                _context.Memberships.Remove(memberships);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MembershipsExists(int id)
        {
            return _context.Memberships.Any(e => e.Id == id);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateMembership(string userId, int planId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                // Update membership plan
                var membership = await _context.Memberships.FirstOrDefaultAsync(m => m.UserId == user.Id);
                membership.MembershipId = planId;
                _context.Memberships.Update(membership);
                await _context.SaveChangesAsync();

                // Update roles based on plan selection
                if (planId == 1) // Free plan
                {
                    await _userManager.RemoveFromRoleAsync(user, "Member");
                    await _userManager.AddToRoleAsync(user, "Visitor");
                }
                else if (planId == 2) // Paid plan
                {
                    await _userManager.RemoveFromRoleAsync(user, "Visitor");
                    await _userManager.AddToRoleAsync(user, "Member");
                }
            }

            return RedirectToAction("Index"); // Or another appropriate view
        }

        public IActionResult Payment(string plan)
        {
            int planId = plan == "Basic" ? 1 : 2;
            string userId = _userManager.GetUserId(User);

            return RedirectToAction("UpdateMembership", new { userId = userId, planId = planId });
        }
    }
}
}
