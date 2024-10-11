using System.ComponentModel.DataAnnotations;

namespace VictuZReviewsAPI.Models
{
    public class ActivityAttendee
    {
        [Key]
        public int Id { get; set; }
        public int ActivityId { get; set; }
        public int AttendeeId { get; set; }
        public Activity Activity { get; set; }
        public Attendee Attendee { get; set; }

    }
}
