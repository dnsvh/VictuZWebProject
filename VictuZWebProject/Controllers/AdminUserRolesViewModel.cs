using VictuZWebProject.Pages.Identity;

namespace VictuZWebProject.Controllers
{
    internal class AdminUserRolesViewModel
    {
        public string SearchString { get; set; }
        public List<ManageUserRolesModel.UserRoleViewModel> UsersWithRoles { get; set; }
    }
}