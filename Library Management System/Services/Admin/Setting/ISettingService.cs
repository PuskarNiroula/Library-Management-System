namespace Library_Management_System.Services.Admin.Setting;

public interface ISettingService
{
    public Task UpdateFineRate(int value);
    public Task UpdateMaxAllowedDays(int value);
    public Task UpdateMaxAllowedBooks(int value);
    
    public Task<Models.DbModels.Setting?> GetSetting(string settingName); 
    
}