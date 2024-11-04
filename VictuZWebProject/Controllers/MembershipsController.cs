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
using VictuZWebProject.Data;
using VictuZWebProject.Areas.Identity.Data;

namespace VictuZWebProject.Controllers
{
    public class MembershipsController : Controller
    {
        private readonly VictuZ_Lars_Db _context;
        private readonly VictuZAccountDbContext _usersContext;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly VictuZAccountDbContext _UserContext;

        public MembershipsController(VictuZ_Lars_Db context, VictuZAccountDbContext usersContext, UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, VictuZAccountDbContext userContext)
        {
            _context = context;
            _usersContext = usersContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _UserContext = userContext;
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

        private bool MembershipsExists(int id)
        {
            return _context.Memberships.Any(e => e.Id == id);
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

        [HttpPost]
        public async Task<IActionResult> AssignVisitorRole()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the user is already in the "Visitor" role
            if (await _userManager.IsInRoleAsync(user, "Visitor"))
            {
                TempData["Message"] = "User is already a visitor.";
                return RedirectToAction("Index", "Home");
            }

            // Assign the "Visitor" role if not already assigned
            await UpdateUserRole(userId, "Visitor");
            TempData["Message"] = "Membership updated successfully.";
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public async Task<IActionResult> AssignMemberRole()
        {
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Unauthorized();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found.");
            }

            // Check if the user is already in the specified role
            if (await _userManager.IsInRoleAsync(user, "Member"))
            {
                // Optionally, return a message or log that the user already has this role
                TempData["Message"] = "User is already a member.";
                return RedirectToAction("Index", "Home");
            }

            // Assign the new role if not already assigned
            await UpdateUserRole(userId, "Member");
            TempData["Message"] = "Membership updated successfully.";
            return RedirectToAction("Index", "Home");
        }



        private async Task UpdateUserRole(string userId, string newRole)
    {
        // Find the user by their ID
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            throw new Exception("User not found.");
        }


        // Get the current roles of the user
        var currentRoles = await _userManager.GetRolesAsync(user);

        // Remove all current roles
        await _userManager.RemoveFromRolesAsync(user, currentRoles);

        // Assign the new role to the user
        await _userManager.AddToRoleAsync(user, newRole);

        // Save the changes
        await _context.SaveChangesAsync();
    }
}

        }


