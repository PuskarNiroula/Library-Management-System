using Library_Management_System.Enum;
using Library_Management_System.Services.Admin.Setting;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Settings/{Action}")]
public class SettingController(ISettingService settingService):Controller
{
    
    public async Task<IActionResult> FineSetup()
    {
        ViewBag.FineRate = await settingService.GetSetting(SettingKeyEnum.FineRate);
        return View("Fine");
    }

    public async Task<IActionResult> MaxBooksSetup()
    {
        ViewBag.MaxBooksAllowed = await settingService.GetSetting(SettingKeyEnum.MaxBooksAllowed);
        return View("MaxBooksSetup");
    }

    public async Task<IActionResult> MaxDaysSetup()
    {
        ViewBag.MaxAllowedDays = await settingService.GetSetting(SettingKeyEnum.MaxAllowedDays);
        return View("MaxDaysSetup");
    }
    
}