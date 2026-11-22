namespace Library_Management_System.Data.Seeder;
public static class Seeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        await SettingSeeder.SeedAsync(context);
        
        await UserSeeder.SeedAsync(context);
    }
}