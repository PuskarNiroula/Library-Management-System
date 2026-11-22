using System.Security.Claims;
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
       
    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new UnauthorizedAccessException();
            await service.CheckoutAsync(checkoutDto, int.Parse(userId));

            return Ok(new
            {
                message = "Checkout completed successfully."
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = ex.Message
            });
        }
    }
    
           [HttpGet("{id:int}")]
    public async Task<IActionResult> Checkin(int id)
    {

        try
        {
            return Ok(await service.GetRentedBookLogs(id));
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                status = "error",
                message = ex.Message
            });
        }

    }

    
}