using Library_Management_System.Data;
using Library_Management_System.Enum;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services.Admin.Setting;

public class SettingService(ApplicationDbContext dbContext):ISettingService
{
    public async Task UpdateFineRate(int value)
    {
        ValidateValue(value);
      var settings=  await dbContext.Settings.Where(s => s.Key ==SettingKeyEnum.FineRate).FirstOrDefaultAsync();
      if(settings==null)
          throw new System.Exception("Settings not found");
      settings.Value = value.ToString();
      await dbContext.SaveChangesAsync();

    }

    public async Task UpdateMaxAllowedDays(int value)
    {
        ValidateValue(value);
        var settings=  await dbContext.Settings.Where(s => s.Key ==SettingKeyEnum.MaxAllowedDays).FirstOrDefaultAsync();
        if(settings==null)
            throw new System.Exception("Settings not found");
        settings.Value = value.ToString();
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateMaxAllowedBooks(int value)
    {
        ValidateValue(value);
        var settings=  await dbContext.Settings.Where(s => s.Key ==SettingKeyEnum.MaxBooksAllowed).FirstOrDefaultAsync();
        if(settings==null)
            throw new System.Exception("Settings not found");
        settings.Value = value.ToString();
        await dbContext.SaveChangesAsync();
    }

    public async Task<Models.DbModels.Setting?> GetSetting(string settingName)
    {
        var settings= await dbContext.Settings.Where(s => s.Key == settingName).FirstOrDefaultAsync();
        return settings ?? throw new System.Exception("Settings not found");
    }

    private void ValidateValue(int value)
    {
        if(value<0)
            throw new System.Exception("Value cannot be negative");
        
    }
}