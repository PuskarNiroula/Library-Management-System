
using Library_Management_System.Data;
using Library_Management_System.Services.Admin.Enum;
using Library_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers.Admin;

public class RequestController (ApplicationDbContext dbContext):Controller
{
   
    /// <summary>
    /// Displays the "Index" view for managing requests; access is restricted to users in the "Admin" role.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> that renders the "Index" view.</returns>
    [Authorize(Roles = "Admin")] 
    public async Task<IActionResult> Index()
    {
        var model = await dbContext.BookRequests
            .Where(r=>r.Status == BookRequestStatusEnum.Pending)
            .Select(r => new BookRequestViewModel
            {
                BookRequestId = r.BookRequestId,
                RequestType = r.RequestType,
                StudentName = r.User.FullName,
                BookName = r.Book.BookName,
                BookImageUrl = r.Book.ImageUrl,
                Status = r.Status
            })
            .ToListAsync();

        return View($"~/Views/Admin/Request/Index.cshtml",model);
    }
    
}