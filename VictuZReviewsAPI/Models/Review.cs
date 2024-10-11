using System.ComponentModel.DataAnnotations;

namespace VictuZReviewsAPI.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public Reviewer Reviewer { get; set; }
        public Activity Activity { get; set; }
    }
}
