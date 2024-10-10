namespace VictuZWebProject.Models
{
    public class SuggestionLike
    {
        public int Id { get; set; }
        public int SuggestionId { get; set; }
        public Suggestion Suggestion { get; set; }
        public string UserId { get; set; }
    }
}
