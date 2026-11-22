using Library_Management_System.Data;
using Library_Management_System.Models;
using Library_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers.Admin;
[Authorize (Roles = "Admin")]
public class BookController(ApplicationDbContext dbContext):Controller
{
    /// <summary>
    /// Displays the admin book index page and sets the active menu to "Book".
    /// </summary>
    /// <returns>An IActionResult that renders the Admin Book Index view populated with the list of books (including their Category data).</returns>
 
    public async Task<IActionResult> Index(int page = 1)
    {
        const int pageSize = 10;

        ViewBag.ActiveMenu = "Book";

        var query = dbContext.Books
            .AsNoTracking()
            .Include(b => b.Category)
            .OrderBy(b => b.BookName);

        var totalBooks = await query.CountAsync();

        var totalPages = (int)Math.Ceiling(totalBooks / (double)pageSize);

        page = Math.Max(1, Math.Min(page, Math.Max(1, totalPages)));

        var books = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var viewModel = new BooksViewModel
        {
            Books = books,
            CurrentPage = page,
            TotalPages = totalPages
        };

        return View("~/Views/Admin/Book/Index.cshtml", viewModel);
    }


    /// <summary>
    /// Displays the admin book creation view and provides the list of categories for the form.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> that renders the Admin/Book Create view with the list of categories as the model.</returns>
    public IActionResult Create()
    {
        ViewBag.ActiveMenu="Book";
        var  categories = dbContext.Category.ToList();
        return View("~/Views/Admin/Book/Create.cshtml",categories);
    }

    /// <summary>
    /// Displays the edit form for the book with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the book to edit.</param>
    /// <returns>An <see cref="IActionResult"/> containing the Edit view with an <see cref="EditBookView"/> model when the book exists, or a NotFound result with an error object when the book does not exist.</returns>
    public async Task<IActionResult> Edit(int id)
    {
        ViewBag.ActiveMenu="Book";
        var book=await dbContext.Books.FindAsync(id);
        if (book==null) return NotFound(
        new
        {
            status = "error",
            message = "Book does not exist"
        });
        var categories = dbContext.Category.ToList();
        var model = new EditBookView
        {
            Book = book,
            Categories = categories
        };
     
        return View("~/Views/Admin/Book/Edit.cshtml",model);
    }
    
}