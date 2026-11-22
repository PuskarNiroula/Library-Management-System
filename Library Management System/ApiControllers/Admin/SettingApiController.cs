using Library_Management_System.Services.Admin.Setting;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.ApiControllers.Admin;

[ApiController]
[Route("api/setting-api")]
public class SettingApiController(ISettingService settingService) : ControllerBase
{
    [HttpPost("update-fine")]
    public async Task<IActionResult> UpdateFine([FromBody] int value)
    {
        if (value <= 0)
            return BadRequest("Fine rate cannot be negative");

        try
        {
            await settingService.UpdateFineRate(value);
            return Ok("Fine rate updated successfully");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("update-max-books")]
    public async Task<IActionResult> UpdateMaxBooksAllowed([FromBody] int value)
    {
        if (value <= 0)
            return BadRequest("Maximum books cannot be negative");

        try
        {
            await settingService.UpdateMaxAllowedBooks(value);
            return Ok("Book limit updated successfully");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }

    [HttpPost("update-max-days")]
    public async Task<IActionResult> UpdateMaxAllowedDays([FromBody] int value)
    {
        if (value <= 0)
            return BadRequest("Maximum allowed days cannot be negative");

        try
        {
            await settingService.UpdateMaxAllowedDays(value);
            return Ok("Days limit updated successfully");
        }
        catch (Exception e)
        {
            return StatusCode(500, e.Message);
        }
    }
}