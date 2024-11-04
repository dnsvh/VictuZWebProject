using static VictuZWebProject.Pages.Identity.ManageUserRolesModel;

namespace VictuZWebProject.Models
{
    public class AdminUserRolesViewModel
    {
        public string SearchString { get; set; }
        public List<UserRoleViewModel> UsersWithRoles { get; set; }
    }

}
