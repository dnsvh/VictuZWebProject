using System.ComponentModel.DataAnnotations;

namespace VictuZReviewsAPI.Models
{
    public class Attendee
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<ActivityAttendee> ActivityAttendees { get; set; }
    }
}
