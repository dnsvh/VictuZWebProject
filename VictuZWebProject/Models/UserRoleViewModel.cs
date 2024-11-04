using VictuZWebProject.Areas.Identity.Data;

namespace VictuZWebProject.Models
{
    public class UserRoleViewModel
    {
        public AppUser User { get; set; }
        public IList<string> Roles { get; set; }
    }
}
