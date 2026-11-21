using Library_Management_System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers.Admin;
[Authorize(Roles = "Admin")]
[Route("/Circulation/CheckOut")]
public class CheckoutController(ApplicationDbContext context):Controller
{

    public async Task<IActionResult> Index()
    {
        var students = await context.Users
            .Where(s => s.Role == "Student")
            .ToListAsync();

        var books = await context.Books
            .Where(b => b.BookInventory.AvailableQuantity > 0)
            .ToListAsync();

        ViewBag.Students = students;
        ViewBag.Books = books;

        return View("Index");
    }
    
}