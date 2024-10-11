using VictuZWebProject.Areas.Identity.Data;

namespace VictuZWebProject.Models
{
    public class Suggestion
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string ?Author { get; set; }
        public string Body { get; set; }
        public int Likes { get; set; }

    }
}
