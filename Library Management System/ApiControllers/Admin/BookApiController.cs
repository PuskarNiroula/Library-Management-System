using Library_Management_System.Data;
using Library_Management_System.DTOs.Book;
using Library_Management_System.Services.Admin.Book;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Library_Management_System.ApiControllers.Admin;
[ApiController]
[Route("api/book-api/[action]")]
[Authorize(Roles = "Admin")]
[AutoValidateAntiforgeryToken]
public class BookApiController(IBookService service,ApplicationDbContext dbContext) :ControllerBase {
    /// <summary>
    /// Creates a new book record, persists the uploaded cover image to disk, and returns an HTTP response indicating the outcome.
    /// </summary>
    /// <param name="dto">Data for the new book. Must include non-empty BookName and Author, Quantity greater than or equal to 0, CategoryId greater than or equal to 0, and an Image file with extension .jpg, .jpeg, .png, or .webp whose size is greater than 0 and no more than 2 MB. On success the DTO's ImageUrl is set to the saved image path.</param>
    /// <returns>An IActionResult representing the HTTP response: 200 OK with a success message when creation succeeds; 400 Bad Request when input validation fails; 500 Internal Server Error when creation fails.</returns>
    [HttpPost]
    
    public async Task<IActionResult> Create(CreateBookDto dto) {
        
        if (string.IsNullOrEmpty(dto.BookName))
                return BadRequest(new
                {
                    status = "error",
                    message = "bookname cannot be empty",
                });

        if (string.IsNullOrEmpty(dto.Author))
            return BadRequest(new
            {
                status = "error",
                message = "author cannot be empty",
            });
        
        if (dto.CategoryId < 0)
            return BadRequest(new
            {
                status = "error",
                message = "select a valid category",
            });
        
        var validExtension = new List<string> { ".jpg", ".jpeg", ".png", "webp" };
        var extension=Path.GetExtension(dto.Image?.FileName)?.ToLower();
        
        if (extension != null && !validExtension.Contains(extension))
            return BadRequest(new
            {
                status = "error",
                message = "Only JPG, JPEG, and WEBP image formats are allowed"
            });
        
        if (dto.Image.Length is > 2 * 1024 * 1024 or 0)
            return BadRequest(new
            {
                status = "error",
                message = "Image must be a valid and no more than 2mb"
            });
        
        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Books");
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);
        var uniqueFileName = Guid.NewGuid() + extension;
        var filePath = Path.Combine(path, uniqueFileName);
        await using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await dto.Image.CopyToAsync(stream); 
        }
        dto.ImageUrl ="/Images/Books/"+uniqueFileName;
        
        if (await service.CreateBookAsync(dto))
        {
            return Ok(new
            {
                status = "success",
                message = "Book created"
            });
        }

        return StatusCode(500,
            new
            {
                status = "error",
                message = "Book creation failed"
            });
    }
    /// <summary>
    /// Updates an existing book's data, optionally replacing its image when provided.
    /// </summary>
    /// <param name="dto">DTO containing the book's identifier and updated fields; include Image to replace the stored book image (optional).</param>
    /// <returns>
    /// An IActionResult representing the outcome:
    /// - 200 OK with a success message when the book is updated;
    /// - 400 BadRequest with error details for invalid input;
    /// - 500 Internal Server Error when the update fails on the server side.
    /// </returns>
    [HttpPut]
    public async Task<IActionResult> Update( UpdateBookDto dto)
    {
        if (dto.BookId <1)
            return BadRequest(new
            {
                status = "error",
                message = "Invalid book id"
            });
        if (string.IsNullOrEmpty(dto.BookName))
            return BadRequest(new
            {
                status = "error",
                message = "bookname cannot be empty",
            });

        if (string.IsNullOrEmpty(dto.Author))
            return BadRequest(new
            {
                status = "error",
                message = "author cannot be empty",
            });
        
        if (dto.CategoryId < 0)
            return BadRequest(new
            {
                status = "error",
                message = "select a valid category",
            });
        if (dto.Image != null)
        {
            var validExtension = new List<string> { ".jpg", ".jpeg", ".png", "webp" };
            var extension = Path.GetExtension(dto.Image.FileName).ToLower();
            if (!validExtension.Contains(extension))
                return BadRequest(new
                    { status = "error", message = "Only JPG, JPEG, and WEBP image formats are allowed" });
            if (dto.Image.Length > 2 * 1024 * 1024)
                return BadRequest(new { status = "error", message = "Image size must not exceed 2MB" });
            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images/Books");
            var uniqueFileName = Guid.NewGuid().ToString() + extension;
            var filePath = Path.Combine(path, uniqueFileName);

            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.Image.CopyToAsync(stream);
            }

            dto.ImageUrl = "/Images/Books/" + uniqueFileName;
        }

        if (await service.UpdateBookAsync(dto))
            return Ok(new
            {
                status = "success",
                message = "Book Updated Successfully"
            });
        return StatusCode(500, new
        {
            status = "error",
            message = "Internal Server Error just to check"
        });
    }

    /// <summary>
    /// Deletes the book with the specified identifier.
    /// </summary>
    /// <param name="id">The identifier of the book to delete; must be greater than or equal to 1.</param>
    /// <returns>
    /// An <see cref="IActionResult"/> whose response is:
    /// - 200 OK with a success status and confirmation message when the book is deleted;
    /// - 400 Bad Request with an error status and "Invalid book id" when <paramref name="id"/> is less than 1;
    /// - 500 Internal Server Error with an error status when deletion fails.
    /// </returns>
    [HttpDelete]
    [Route("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (id < 1)
            return BadRequest(new
            {
                status = "error",
                message = "Invalid book id"
            });
        if (await service.DeleteBookAsync(id))
            return Ok(new
            {
                status = "success",
                message = "Book Deleted Successfully",
            });
        return StatusCode(500, new
        {
            status = "error",
            message = "Internal Server Error just to check"
        });
    }

 

}