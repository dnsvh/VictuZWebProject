using System.ComponentModel.DataAnnotations;

namespace VictuZWebProject.Models
{
    public class Memberships
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string MembershipId { get; set; }
    }
}
