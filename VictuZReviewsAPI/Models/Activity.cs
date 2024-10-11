using System.ComponentModel.DataAnnotations;
namespace VictuZReviewsAPI.Models
{
    public class Activity
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public Host Host { get; set; }
        public ICollection<Review> Reviews { get; set; }
        public ICollection<ActivityAttendee> ActivityAttendees { get; set; }
    }
}
