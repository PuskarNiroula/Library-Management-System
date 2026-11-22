using Library_Management_System.Helpers;
using Library_Management_System.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Data.Seeder;

public static class UserSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var users = new List<User>
        {
            new User
            {
                FullName = "Puskar Niroula",
                Email = "puskar@gmail.com",
                PasswordHash = PasswordHelper.HashPassword("Newlife@2057"),
                Role = "Admin"
            },
            new User
            {
                FullName = "Samana Dahahl",
                Email = "samana@gmail.com",
                PasswordHash = PasswordHelper.HashPassword("Newlife@2057"),
                Role = "Student"
            }
        };

        foreach (var user in users)
        {
            bool exists = await context.Users
                .AnyAsync(x => x.Email == user.Email);

            if (!exists)
            {
                context.Users.Add(user);
            }
        }

        await context.SaveChangesAsync();
    }
}
