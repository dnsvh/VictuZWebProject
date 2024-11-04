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


