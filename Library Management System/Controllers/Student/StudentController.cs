using System.Security.Claims;
using Library_Management_System.Services.Admin;
using Library_Management_System.Services.Admin.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers.Student;

[Route("Student")]
[Authorize(Roles = "Student")]
public class StudentController(IBookService service,CirculationService circulationService):Controller{

    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Student";
        var newBooks= await service.GetNewBooks();
        return View("Index",newBooks);
    }

    [Route("Rentals")]
    public async Task<IActionResult> Rentals()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return Unauthorized();

        try
        {
            var rentals = await circulationService.GetRentedLogs(int.Parse(userId));
            return View("Rentals", rentals);


        }
        catch (Exception e)
        {
            return BadRequest(e.Message);
        }
    }
}