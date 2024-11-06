using System;
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
        public DbSet<Memberships> Memberships { get; set; }
        public DbSet<Store> Store { get; set; }
        public DbSet<Category> Categories { get; set; } // Add this line


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string connection = @"Data Source=.;Initial Catalog=VictuzWebsite-2;Integrated Security=true;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(connection);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Activity>().ToTable("Activity");
            modelBuilder.Entity<UserRegistration>().ToTable("UserRegistration");
            modelBuilder.Entity<Suggestion>().ToTable("Suggestion");
            modelBuilder.Entity<SuggestionLike>().ToTable("SuggestionLike");
            modelBuilder.Entity<Memberships>().ToTable("Memberships");

            // Update precision for Price column in Store
            modelBuilder.Entity<Store>()
                .Property(s => s.Price)
                .HasPrecision(10, 2); // Increased precision

            base.OnModelCreating(modelBuilder);
        }
    }
}
