using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Threading.Tasks;
using System.Collections.Generic;
using VictuZWebProject.Areas.Identity.Data;

namespace VictuZWebProject.Areas.Identity.Pages.Account
{
    [Authorize(Roles = "Staff, Admin")]
    public class ManageRolesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageRolesModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public List<UserWithRoles> UsersWithRoles { get; set; }
        public List<string> Roles { get; set; }

        [BindProperty]
        public string SelectedRole { get; set; }

        [BindProperty]
        public string UserId { get; set; }

        public async Task OnGetAsync()
        {
            // Get all users and their roles
            UsersWithRoles = new List<UserWithRoles>();
            var users = _userManager.Users;

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                UsersWithRoles.Add(new UserWithRoles
                {
                    User = user,
                    Roles = roles
                });
            }

            // Define all the roles that can be assigned
            Roles = new List<string> { "Pending", "Visitor", "Member", "Staff", "Admin" };
        }

        public async Task<IActionResult> OnPostChangeRoleAsync()
        {
            var user = await _userManager.FindByIdAsync(UserId);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return Page();
            }

            // Check if the user making the request is Admin or Staff
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserRoles = await _userManager.GetRolesAsync(currentUser);

            if (currentUserRoles.Contains("Admin"))
            {
                // Admin can assign any role up to "Staff"
                if (SelectedRole == "Admin" || SelectedRole == "Staff")
                {
                    await UpdateUserRole(user, SelectedRole);
                }
            }
            else if (currentUserRoles.Contains("Staff"))
            {
                // Staff can only assign from "Pending" to "Visitor"
                if (SelectedRole == "Visitor" && (await _userManager.IsInRoleAsync(user, "Pending")))
                {
                    await UpdateUserRole(user, SelectedRole);
                }
            }

            return RedirectToPage();
        }

        private async Task UpdateUserRole(AppUser user, string newRole)
        {
            // Remove user from all roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add user to the selected new role
            await _userManager.AddToRoleAsync(user, newRole);
        }

        public class UserWithRoles
        {
            public AppUser User { get; set; }
            public IList<string> Roles { get; set; }
        }
    }
}

