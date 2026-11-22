
using Library_Management_System.Enum;
using Library_Management_System.Models.DbModels;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Data.Seeder;

public static class SettingSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        var settings = new List<Setting>
        {
            new Setting
            {
                Key = SettingKeyEnum.FineRate,
                Value = "10"
            },
            new Setting
            {
                Key = SettingKeyEnum.MaxAllowedDays,
                Value = "15"
            },
            new Setting
            {
                Key = SettingKeyEnum.MaxBooksAllowed,
                Value = "5"
            }
        };

        foreach (var setting in settings)
        {
            bool exists = await context.Settings
                .AnyAsync(x => x.Key == setting.Key);

            if (!exists)
            {
                context.Settings.Add(setting);
            }
        }

        await context.SaveChangesAsync();
    }
}