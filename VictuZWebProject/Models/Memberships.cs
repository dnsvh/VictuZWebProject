using System.ComponentModel.DataAnnotations;

namespace VictuZWebProject.Models
{
    public class Memberships
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public int MembershipId { get; set; }
    }
}
