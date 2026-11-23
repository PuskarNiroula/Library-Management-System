using System.Security.Claims;
using Library_Management_System.Data;
using Library_Management_System.Services.Admin.Enum;
using Library_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers.Student;

[Authorize(Roles = "Student")]
[Route("Student/[action]")]
public class RequestController(ApplicationDbContext context):Controller
{[HttpGet]
    public async Task<IActionResult> Requests()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!int.TryParse(userIdClaim, out var userId))
            return Unauthorized();
        

        var request = await context.BookRequests
            .Where(br => br.Status != BookRequestStatusEnum.Cancelled &&
                         br.Status != BookRequestStatusEnum.Approved &&
                         br.Status != BookRequestStatusEnum.Returned)
            .Where(b => b.UserId == int.Parse(userIdClaim))
            .Include(br => br.Book)
            .Select(s => new BookRequestViewModel()
            {
                BookName = s.Book.BookName,
                BookImageUrl = s.Book.ImageUrl,
                RequestType = s.RequestType,
                Status = s.Status,
                BookRequestId = s.BookRequestId
            })
            .ToListAsync();

        return View("~/Views/Student/BookRequest/Index.cshtml", request);
    }

    
}