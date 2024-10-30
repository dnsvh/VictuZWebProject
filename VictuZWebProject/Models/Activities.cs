namespace VictuZ_Lars.Models
{
    public class Activity
    {
        public int ActivityId { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string Location { get; set; }
        public string ?ImageUrl { get; set; }
        public int Registered { get; set; }
        public int MaxCapacity { get; set; }
        public DateTime DatePublished { get; set; }
        public DateTime DateDue { get; set; }

        public bool OnlyMembers { get; set; }//Alleen members kunnen zich aanmelden
        //Kan zijn dat members zich eerder kunnen aanmelden
        public DateTime? MembersOnlyVisibilityEnd { get; set; }// Tijd eindigt voor members only
        
        //Members plekken reserveren binnen een activiteit
        public bool MembersPreRegistration { get; set; }//Plekken voor members reserveren
        public int MembersOnlyCapacity { get; set; }//Hoeveel plekken er voor members zijn gereserveerd


    }
}
