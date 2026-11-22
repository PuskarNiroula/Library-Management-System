using Library_Management_System.Data;
using Library_Management_System.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers.Admin;

[Authorize(Roles = "Admin")]
[Route("Setting/{Action}")]
public class SettingController(ApplicationDbContext dbContext):Controller
{
    
    public async Task<IActionResult> Fine()
    {
        var Fine = await dbContext.Settings.Where(s => s.Key == SettingKeyEnum.FineRate).FirstOrDefaultAsync();
        return View("Fine", Fine);

    }
    
    
}