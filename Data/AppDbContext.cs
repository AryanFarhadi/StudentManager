using AKP.Models;
using Microsoft.EntityFrameworkCore;

namespace AKP.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students => Set<Student>();
        public DbSet<Major> Majors => Set<Major>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure StudentNumber is unique
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentNumber)
                .IsUnique();

            // Ensure Major Title is unique
            modelBuilder.Entity<Major>()
                .HasIndex(m => m.Title)
                .IsUnique();

            // Relationship: Major has many Students, restrict delete if students exist
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Major)
                .WithMany(m => m.Students)
                .HasForeignKey(s => s.MajorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
