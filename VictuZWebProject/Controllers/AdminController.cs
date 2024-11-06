using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VictuZWebProject.Areas.Identity.Data;
using VictuZWebProject.Models;
using static VictuZWebProject.Pages.Identity.ManageUserRolesModel;
using UserRoleViewModel = VictuZWebProject.Models.UserRoleViewModel;
//using UserRoleViewModel = VictuZWebProject.Pages.Identity.ManageUserRolesModel.UserRoleViewModel;

namespace VictuZWebProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }


        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Index(string searchString)
        {
            var usersWithRoles = await GetUsersWithRoles(searchString);
            var model = new AdminUserRolesViewModel
            {
                SearchString = searchString,
                UsersWithRoles = usersWithRoles
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> EditRole(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, newRole);

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin,Staff")]
        private async Task<List<UserRoleViewModel>> GetUsersWithRoles(string searchString)
        {
            var users = _userManager.Users;
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.Email.Contains(searchString));
            }

            var userRoles = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(new UserRoleViewModel
                {
                    User = user,
                    Roles = roles
                });
            }

            return userRoles;
        }

        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> PendingUsers()
        {
            var pendingUsers = await GetPendingUsers();
            var model = new PendingUsersViewModel
            {
                PendingUsers = pendingUsers
            };
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> AcceptPendingUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            // Remove all roles and assign the "Visitor" role
            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);
            await _userManager.AddToRoleAsync(user, "Visitor");

            return RedirectToAction("PendingUsers");
        }

        private async Task<List<UserRoleViewModel>> GetPendingUsers()
        {
            var users = await _userManager.Users.ToListAsync();
            var pendingUsers = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                if (roles.Contains("Pending"))
                {
                    pendingUsers.Add(new UserRoleViewModel
                    {
                        User = user,
                        Roles = roles
                    });
                }
            }

            return pendingUsers;
        }

    }

}
