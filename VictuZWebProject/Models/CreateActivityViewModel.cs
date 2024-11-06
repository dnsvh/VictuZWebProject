namespace VictuZWebProject.Models
{
    public class CreateActivityViewModel
    {
        public int SuggestionId { get; set; }
        public string Name { get; set; }
        public string Body { get; set; }
        public string Organizer { get; set; }
        public string Location { get; set; }
        public int MaxCapacity { get; set; }
        public DateTime DateDue { get; set; }
    }
}
