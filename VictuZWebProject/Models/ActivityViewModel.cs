using VictuZ_Lars.Models;

namespace VictuZWebProject.Models
{
    public class ActivityViewModel
    {
        public Activity Activity { get; set; }
        public bool IsUserRegistered { get; set; }  // Indicates if the current user is registered for the activity
        public int AvailableForMembers { get; set; }
        public int AvailableForNonMembers { get; set; }


    }


}
