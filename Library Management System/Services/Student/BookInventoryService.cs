using Library_Management_System.Data;
using Library_Management_System.DTOs.Book;
using Library_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services.Student;

public class BookInventoryService(ApplicationDbContext dbContext) : IBookInventoryService
{
    public async Task<bool> RequestBookAsync(BookRequestDto dto)
    {
        var bookInventory = await dbContext.BookInventories
            .FirstOrDefaultAsync(x => x.BookId == dto.BookId);

        if (bookInventory == null)
            throw new Exception("Book not found");

        if (bookInventory.AvailableQuantity <= 0)
            throw new Exception("Not enough quantity available");

        bookInventory.AvailableQuantity--;

        var request = new BookRequest
        {
            BookId = dto.BookId,
            RequestType = dto.RequestType,
            UserId = dto.UserId
        };

        await dbContext.BookRequests.AddAsync(request);

        return await dbContext.SaveChangesAsync() > 0;
    }
}