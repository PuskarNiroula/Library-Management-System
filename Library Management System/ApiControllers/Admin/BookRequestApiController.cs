using System.Security.Claims;
using Library_Management_System.Data;
using Library_Management_System.DTOs.Checkout;
using Library_Management_System.Enum;
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
    
       [HttpGet("{id:int}")]
    public async Task<IActionResult> Checkin(int id)
    {

        try
        {

            return Ok(await GetRentedBookLogs(id));
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

    private async Task<List<CheckInResponseViewModel>> GetRentedBookLogs(int userId)
    {
        int maxDays = await GetMaxAllowedDays();
        int fineRate = await GetFineRate();
        DateTime today = DateTime.Today;

        var rentedBooks = await context.RentLogs.AsNoTracking()
            .Where(l => l.StudentId == userId && l.Status == RentLogStatusEnum.Active)
            .Select(a => new
            {
                a.RentLogId,
                a.BookId,
                BookName = a.Book.BookName,
                a.RentedDate,
                Price = a.Book.Price
            })
            .ToListAsync();

        return rentedBooks.Select(a =>
        {
            DateTime maxAllowedDate = a.RentedDate.AddDays(maxDays);

            return new CheckInResponseViewModel
            {
                RentedLogId = a.RentLogId,
                BookId = a.BookId,
                BookName = a.BookName,
                RentedDate = a.RentedDate,
                price = a.Price,
                MaxAllowedDate = maxAllowedDate,
                Fine = today > maxAllowedDate
                    ? fineRate * (today - maxAllowedDate).Days
                    : 0
            };
        }).ToList();
    }

    private async Task<int> GetFineRate()
    {
      var rate = await context.Settings.Where(s => s.Key == SettingKeyEnum.FineRate)
          .Select(s => s.Value)
          .FirstOrDefaultAsync();
      
      if (rate == null)
          throw new Exception("Fine rate is not set please set before checking in any student");
      
      return int.Parse(rate);
    }

    private async Task<int> GetMaxAllowedDays()
    {
        var allowedDays = await context.Settings.Where(s => s.Key == SettingKeyEnum.MaxAllowedDays)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();
        
        if (allowedDays==null)
            throw new Exception("Fine rate is not set please set before checking in any student");
        
        return int.Parse(allowedDays);
    }
}