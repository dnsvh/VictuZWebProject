using VictuZ_Lars.Models;
using VictuZWebProject.Areas.Identity.Data;
namespace VictuZWebProject.Models
{
    public class UserRegistration
    {
        public int Id { get; set; }
        public string UserId { get; set; }  // Identity user ID
        public int ActivityId { get; set; }
        public Activity Activity { get; set; } 
        public AppUser AppUser { get; set; } 
    }
}
