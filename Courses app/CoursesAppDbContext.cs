using Courses_app.Models;
using Microsoft.EntityFrameworkCore;

namespace Courses_app
{
    public class CoursesAppDbContext : DbContext
    {
        public CoursesAppDbContext(DbContextOptions<CoursesAppDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<BasicUser> BasicUsers { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<Video> Video { get; set; }
        public DbSet<Course> Course { get; set; } 
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Category> Category { get; set; }
        public DbSet<CategoryGroup> CategoryGroup { get; set; }
        public DbSet<Rating > Rating { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<Rating>()
            .HasOne(r => r.Course)
            .WithMany(c => c.Ratings)
            .HasForeignKey(r => r.CourseId);

            modelBuilder.Entity<Rating>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId);
        }
    }
}
