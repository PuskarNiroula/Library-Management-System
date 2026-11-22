using Library_Management_System.DTOs.Checkout;
using Library_Management_System.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.ApiControllers.Admin;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/circulation-api/[action]")]
public class CirculationApiController(CirculationService service):ControllerBase
{
    public async Task<IActionResult> CheckIn([FromBody] CheckInRequestDto request)
    {

        try
        {
            await service.CheckIn(request);
            return Ok(new
            {
                status = "success",
                message = "Successfully checked In"
            });
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        
    }
    
}