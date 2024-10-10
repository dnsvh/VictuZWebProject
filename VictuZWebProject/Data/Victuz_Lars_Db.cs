using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VictuZ_Lars.Models;
using VictuZWebProject.Models;

namespace VictuZ_Lars.Data
{
    public class VictuZ_Lars_Db : DbContext
    {
        public DbSet<Activity> Activity { get; set; }
        public DbSet<UserRegistration> UserRegistration { get; set; }
        public DbSet<Suggestion> Suggestion { get; set; }
        public DbSet<SuggestionLike> SuggestionLike { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection = @"Data Source=.;Initial Catalog=VictuzWebsite;Integrated Security=true;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>().ToTable("Activity");
            modelBuilder.Entity<UserRegistration>().ToTable("UserRegistration");
            modelBuilder.Entity<Suggestion>().ToTable("Suggestion");
            modelBuilder.Entity<SuggestionLike>().ToTable("SuggestionLike");

        }
        public DbSet<Activity> Activity_1 { get; set; } = default!;
        public DbSet<UserRegistration> UserRegistration_1 { get; set; } = default!;
        public DbSet<Suggestion> Suggestion_1 { get; set; } = default!;

        public DbSet<SuggestionLike> SuggestionLike_1 { get; set; } = default!;



    }
}
