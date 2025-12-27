using System.Security.Claims;
using Library_Management_System.Enum;
using Library_Management_System.Models;
using Library_Management_System.Services.Admin.Book;
using Library_Management_System.Services.Admin.Exception;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library_Management_System.ApiControllers.Student;
[ApiController]
[Route("api/student")]
[Authorize(Roles = UserRoleEnum.Student)]
public class BookApiController(IBookService service,IBookRequestService bookRequestService):ControllerBase
{
    private readonly IBookService _service=service ?? throw new ArgumentNullException(nameof(service));
    private readonly IBookRequestService _bookRequestService=bookRequestService ?? throw new ArgumentNullException(nameof(bookRequestService));
   
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

    [HttpPost("buy-book")]
    public async Task<IActionResult> BuyBook([FromForm] int id)
    {
        try
        {
            if (await _service.SaleOrRentBook(id, "sales"))
            {
                return Ok(new
                {
                    status = "success",
                    message = "Request to buy book sent successfully"
                });
            }
            else
            {
                return BadRequest(new
                {
                    status = "failed",
                    message = "Could not process the book purchase request."
                });
            }
        }
        catch (BookNotFoundException exception)
        {
            return NotFound(new
            {
                status = "error",
                message = exception.Message
            });
        }
    }

    [HttpPost("rent-book")]
    public async Task<IActionResult> RentBook([FromForm] int id)
    {
        try
        {
          var book= await _service.GetBookByIdAsync(id);
             BookRequest request = new BookRequest
             {
                 BookId = book.BookId,
                 RequestType = RequestTypeEnum.Rent,
                 UserId = int.TryParse(
                     User.FindFirstValue(ClaimTypes.NameIdentifier),
                     out var userId
                 )? userId : throw new UnauthorizedAccessException("Invalid token")
             };
             if (await _bookRequestService.CreateBookRequestAsync(request))
                     return Ok(new {status = "success"});
                 
             return BadRequest(new {status = "error"});
        }
        catch (BookNotFoundException exception)
        {
            return BadRequest(new
            {
                status = "error",
                message = exception.Message

            });
        }
    }

    
}