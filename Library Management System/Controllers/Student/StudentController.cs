using Library_Management_System.Services.Admin.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.Controllers.Student;

[Route("Student")]
[Authorize(Roles = "Student")]
public class StudentController(IBookService service):Controller
{
    private readonly IBookService _service=service ?? throw new ArgumentNullException(nameof(service));
    public async Task<IActionResult> Index()
    {
        ViewData["ActiveMenu"] = "Student";
        var newBooks= await _service.GetNewBooks();
        return View("Index",newBooks);
    }
    [Route("Requests")]
    public Task<IActionResult> Requests()
    {
        ViewData["ActiveMenu"] = "Requests";
        return Task.FromResult<IActionResult>(View("Requests"));  
    }

    [Route("Rentals")]
    public async Task<IActionResult> Rentals()
    {
        ViewData["ActiveMenu"] = "Rentals";
        return View("Rentals");
    }

    [Route("History")]
    public async Task<IActionResult> History()
    {
        ViewData["ActiveMenu"] = "History";
        return View("History");  
    }
    
    [Route("Profile")]
    public async Task<IActionResult> Profile()
    {
        ViewData["ActiveMenu"] = "Profile";
        return View("Profile/Index");  
    }
}