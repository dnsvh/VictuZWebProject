using System.ComponentModel.DataAnnotations;

namespace VictuZReviewsAPI.Models
{
    public class Host
    {
        [Key]
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<Activity> Activities { get; set; }
    }
}
