using System.Security.Claims;
using Library_Management_System.Helpers;
using Library_Management_System.Services.Admin.BookRequest;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.ApiControllers.Admin;
[ApiController]
[Route("api/book-request-api/[action]")]
[Authorize(Roles = "Admin")]
[AutoValidateAntiforgeryToken]
public class BookRequestApiController(IBookRequestService bookRequestService):ControllerBase
{
    
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Reject(int id)
    {
        try
        {
            await bookRequestService.Reject(id);
            return Ok(
                new
                {
                    status = "success",
                    message = "Book rejected"
                });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex);
            
        }
        
    } 
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Approve(int id)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            await bookRequestService.Approve(id, userId);

            return Ok(new
            {
                status = "success",
                message = "Book approved"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, ex.Message);
        }
    }
}