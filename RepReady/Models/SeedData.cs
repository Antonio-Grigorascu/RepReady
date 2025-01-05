using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using RepReady.Data;

namespace RepReady.Models
{
    public class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {

                if (context.Roles.Any())
                {
                    return;
                }


                context.Roles.AddRange(
                new IdentityRole { Id = "b1e9ddbe-b5fc-495c-ab73-47cf7f8f9130", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                new IdentityRole { Id = "b1e9ddbe-b5fc-495c-ab73-47cf7f8f9131", Name = "Organizer", NormalizedName = "Organizer".ToUpper() },
                new IdentityRole { Id = "b1e9ddbe-b5fc-495c-ab73-47cf7f8f9132", Name = "User", NormalizedName = "User".ToUpper() }
                );

                var hasher = new PasswordHasher<ApplicationUser>();


                context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "70e8a9f2-588d-43bf-9a6a-87d132f66130", // primary key
                    UserName = "admin@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Admin1!")
                },

                new ApplicationUser
                {
                    Id = "70e8a9f2-588d-43bf-9a6a-87d132f66131", // primary key
                    UserName = "organizer@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "ORGANIZER@TEST.COM",
                    Email = "organizer@test.com",
                    NormalizedUserName = "ORGANIZER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Organizer1!")
                },

                new ApplicationUser
                {
                    Id = "70e8a9f2-588d-43bf-9a6a-87d132f66132", // primary key
                    UserName = "user@test.com",
                    EmailConfirmed = true,
                    NormalizedEmail = "USER@TEST.COM",
                    Email = "user@test.com",
                    NormalizedUserName = "USER@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "User1!")
                }
                );


                context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "b1e9ddbe-b5fc-495c-ab73-47cf7f8f9130",
                    UserId = "70e8a9f2-588d-43bf-9a6a-87d132f66130"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "b1e9ddbe-b5fc-495c-ab73-47cf7f8f9131",
                    UserId = "70e8a9f2-588d-43bf-9a6a-87d132f66131"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "b1e9ddbe-b5fc-495c-ab73-47cf7f8f9132",
                    UserId = "70e8a9f2-588d-43bf-9a6a-87d132f66132"
                }
                );

                context.SaveChanges();
            }

        
        }

    }
}
