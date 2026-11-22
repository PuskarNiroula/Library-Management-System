using System.Security.Claims;
using Library_Management_System.DTOs.Book;
using Library_Management_System.Enum;
using Library_Management_System.Models;
using Library_Management_System.Models.DbModels;
using Library_Management_System.Services.Admin.Book;
using Library_Management_System.Services.Admin.Enum;
using Library_Management_System.Services.Student;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.ApiControllers.Student;
[ApiController]
[Route("api/student")]
[Authorize(Roles = UserRoleEnum.Student)]
public class BookApiController(IBookService service,IBookInventoryService bookInventoryService):ControllerBase
{
    private readonly IBookService _service=service ?? throw new ArgumentNullException(nameof(service));
   
   
    /// <summary>
    /// Gets a paginated list of books for the student view.
    /// </summary>
    /// <param name="page">Page number to retrieve; if 0, page 1 is used.</param>
    /// <param name="size">Number of items per page; values less than 1 are set to 6 and values greater than 10 are set to 10.</param>
    /// <returns>An HTTP 200 OK response containing a PaginatedBook&lt;Book&gt; with the requested page of books.</returns>
    [HttpGet("paginated")]
    public async Task<IActionResult> Index(int page, int size)
    {
        if(size<=0) size=6;
        if(size>10) size=10;
        var paginatedBooks = await _service.GetPaginatedBooks(page==0?1:page, size);
        return Ok(paginatedBooks);
    }
    /// <summary>
    /// Retrieves a page of books, either filtered by an optional search term or the unfiltered paginated list.
    /// </summary>
    /// <param name="searchTerm">Optional search term used to filter books; if null or empty, returns the unfiltered list.</param>
    /// <param name="page">Page number to retrieve, starting at 1.</param>
    /// <returns>An <see cref="IActionResult"/> with HTTP 200 OK containing a <see cref="PaginatedBook{Book}"/> for the requested page and search criteria.</returns>
    [HttpGet("search")]
    public async Task<IActionResult> Search(string? searchTerm,int page=1)
    {
        PaginatedBook<Book> books;
        if (string.IsNullOrEmpty(searchTerm))
        {
             books =   await _service.GetPaginatedBooks(page);
            return Ok(books);
        }

        books = await _service.GetSearchedBook(searchTerm, page);
            return Ok(books);
    }
    [Authorize]
    [HttpPost("rent")]
    public async Task<IActionResult> Rent([FromBody] RentBookDto rentBookDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
            return Unauthorized();

        var dto = new BookRequestDto
        {
            BookId = rentBookDto.BookId,
            RequestType = "RENT",
            UserId = int.Parse(userId)
        };

        try
        {
            var result = await bookInventoryService.RequestBookAsync(dto);

            return Ok(result);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                message = ex.Message
            });
        }
        
    }
    [Authorize]
    [HttpPost("buy")]
    public async Task<IActionResult> Buy([FromBody] RentBookDto rentBookDto)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        var dto = new BookRequestDto
        {
            BookId = rentBookDto.BookId,
            RequestType = RequestTypeEnum.Sale,
            UserId = int.Parse(userId)
        };

        try
        {
            var result = await bookInventoryService.RequestBookAsync(dto);

            return Ok(result);
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