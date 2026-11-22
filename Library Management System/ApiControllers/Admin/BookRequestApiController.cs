using System.Security.Claims;
using Library_Management_System.Data;
using Library_Management_System.DTOs.Checkout;
using Library_Management_System.Services.Admin.BookRequest;
using Library_Management_System.Services.Admin.Enum;
using Library_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.ApiControllers.Admin;
[ApiController]
[Route("api/book-request-api/[action]")]
[Authorize(Roles = "Admin")]
[AutoValidateAntiforgeryToken]
public class BookRequestApiController(IBookRequestService bookRequestService,ApplicationDbContext context):ControllerBase
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

    [HttpGet("{id:int}")]
    public async Task<IActionResult> Reservation(int id)
    {
        var model =await context.BookRequests.
            Where(b => b.UserId == id)
            .Where(b=>b.Status==BookRequestStatusEnum.Reserved)
            .Select(m => new ReservationViewModel
        {
            ReservationId = m.BookRequestId,
            BookName = m.Book.BookName,
            BookId = m.BookId,
            Isbn = m.Book.Isbn,
            type = m.RequestType,
            Price = m.Book.Price
        }).ToListAsync();
        
        return Ok(model);
    }
    
   
    [HttpPost]
    public async Task<IActionResult> Checkout([FromBody] CheckoutDto checkoutDto)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
                throw new UnauthorizedAccessException();
            await bookRequestService.CheckoutAsync(checkoutDto, int.Parse(userId));

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
}