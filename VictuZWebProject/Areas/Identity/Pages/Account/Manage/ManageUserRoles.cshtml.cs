using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VictuZWebProject.Areas.Identity.Data;

namespace VictuZWebProject.Pages.Identity
{
    public class ManageUserRolesModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public ManageUserRolesModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // store the user and their roles
        public class UserRoleViewModel
        {
            public AppUser User { get; set; }
            public IList<string> Roles { get; set; }
        }

        // list to store users and their roles
        public IList<UserRoleViewModel> UsersWithRoles { get; set; }
        public string SearchString { get; set; }

        public async Task OnGetAsync(string searchString)
        {
            SearchString = searchString;
            var users = _userManager.Users;

            // search with lambdaa
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.Contains(searchString) || u.Email.Contains(searchString));
            }

            // get user n roles
            UsersWithRoles = new List<UserRoleViewModel>();
            foreach (var user in users.ToList())
            {
                var roles = await _userManager.GetRolesAsync(user);
                UsersWithRoles.Add(new UserRoleViewModel
                {
                    User = user,
                    Roles = roles
                });
            }
        }

        public async Task<IActionResult> OnPostEditRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var currentRoles = await _userManager.GetRolesAsync(user);

            // admin can change all roles, Staff can change roles except Admin and Staff
            if (User.IsInRole("Admin") || (User.IsInRole("Staff") && (newRole != "Admin" && newRole != "Staff")))
            {
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, newRole);
                return RedirectToPage();
            }

            return Forbid();
        }
    }
}


