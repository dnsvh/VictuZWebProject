using Microsoft.EntityFrameworkCore;

namespace VictuZReviewsAPI.Data
{
    public class VictuZContext : DbContext
    {
        public VictuZContext(DbContextOptions<VictuZContext> options) : base(options)
        {
        }

        public DbSet<Models.Reviewer> Reviewers { get; set; }
        public DbSet<Models.Host> Hosts { get; set; }
        public DbSet<Models.Review> Reviews { get; set; }
        public DbSet<Models.Activity> Activities { get; set; }
        public DbSet<Models.ActivityAttendee> ActivityAttendees { get; set; }
        public DbSet<Models.Attendee> Attendees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Models.ActivityAttendee>()
                .HasKey(aa => new { aa.ActivityId, aa.AttendeeId });
            modelBuilder.Entity<Models.ActivityAttendee>()
                .HasOne(aa => aa.Activity)
                .WithMany(a => a.ActivityAttendees)
                .HasForeignKey(aa => aa.ActivityId);
            modelBuilder.Entity<Models.ActivityAttendee>()
                .HasOne(aa => aa.Attendee)
                .WithMany(a => a.ActivityAttendees)
                .HasForeignKey(aa => aa.AttendeeId);
        }
    }
}
