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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Categories
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Strength" },
                new Category { Id = 2, Name = "Cardio" },
                new Category { Id = 3, Name = "Flexibility" }
            );

            // Seed Workouts
            modelBuilder.Entity<Workout>().HasData(
                new Workout
                {
                    Id = 1,
                    Name = "Upper Body Strength",
                    Description = "Workout focusing on upper body strength",
                    Duration = 60,
                    Date = new DateTime(2024, 12, 5),
                    CategoryId = 1,
                    CreatorId = "organizer1"
                },
                new Workout
                {
                    Id = 2,
                    Name = "Morning Cardio",
                    Description = "Morning cardio workout to get your heart pumping",
                    Duration = 45,
                    Date = new DateTime(2024, 12, 6),
                    CategoryId = 2,
                    CreatorId = "organizer2"
                }
            );

            // Seed Exercises
            modelBuilder.Entity<Exercise>().HasData(
                new Exercise
                {
                    Id = 1,
                    Title = "Push-Up",
                    Description = "A standard push-up exercise for chest and triceps.",
                    Reps = 10,
                    Sets = 3,
                    Status = true,
                    Start = new DateTime(2024, 12, 5, 9, 0, 0),
                    Finish = new DateTime(2024, 12, 5, 9, 30, 0),
                    WorkoutId = 1
                },
                new Exercise
                {
                    Id = 2,
                    Title = "Running",
                    Description = "A 5km run for cardio endurance.",
                    Reps = 1,
                    Sets = 1,
                    Status = true,
                    Start = new DateTime(2024, 12, 6, 7, 0, 0),
                    Finish = new DateTime(2024, 12, 6, 7, 45, 0),
                    WorkoutId = 2
                }
            );
        }


    }
}
