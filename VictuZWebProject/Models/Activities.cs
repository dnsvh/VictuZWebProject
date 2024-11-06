using VictuZWebProject.Areas.Identity.Data;

namespace VictuZ_Lars.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string? Organizer { get; set; }
        public string Location { get; set; }
        public string ?ImageUrl { get; set; }
        public int Registered { get; set; }
        public int MaxCapacity { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime DateDue { get; set; }

        public bool OnlyMembers { get; set; } = false;//Alleen members kunnen zich aanmelden

        //Members plekken reserveren binnen een activiteit
        public bool MembersPreRegistration { get; set; } = false;//Plekken voor members reserveren
        //Kan zijn dat members zich eerder kunnen aanmelden
        public DateTime? MembersOnlyVisibilityEnd { get; set; }// Tijd eindigt voor members only

        public bool MembersPriorityCapacity { get; set; } = false;
        public int? MembersOnlyCapacity { get; set; }//Hoeveel plekken er voor members zijn gereserveerd


    }
}
