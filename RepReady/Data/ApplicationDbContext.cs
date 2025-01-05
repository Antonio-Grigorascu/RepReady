using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepReady.Models;

namespace RepReady.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Workout> Workouts { get; set; }

        public DbSet<WorkoutInvitation> WorkoutInvitations { get; set; }

        public DbSet<ApplicationUserExercise> UserExercises { get; set; }

        public DbSet<ExerciseTemplate> ExerciseTemplates { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure the many-to-many relationship using the join entity
            modelBuilder.Entity<ApplicationUserExercise>()
                .HasKey(ue => new { ue.UserId, ue.ExerciseId }); // Composite key

            modelBuilder.Entity<ApplicationUserExercise>()
                .HasOne(ue => ue.User)
                .WithMany(u => u.UserExercises)
                .HasForeignKey(ue => ue.UserId);

            modelBuilder.Entity<ApplicationUserExercise>()
                .HasOne(ue => ue.Exercise)
                .WithMany(e => e.UserExercises)
                .HasForeignKey(ue => ue.ExerciseId);

            base.OnModelCreating(modelBuilder);
        }

    }
}
