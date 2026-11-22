using Library_Management_System.Data;
using Library_Management_System.DTOs.Book;
using Library_Management_System.Enum;
using Library_Management_System.Models.DbModels;
using Library_Management_System.Services.Admin.Enum;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services.Student;

public class BookInventoryService(ApplicationDbContext dbContext) : IBookInventoryService
{
    public async Task<bool> RequestBookAsync(BookRequestDto dto)
    {
        
        var maxAllowedBooksValue = dbContext.Settings
            .Where(s => s.Key == SettingKeyEnum.MaxBooksAllowed)
            .Select(s => s.Value)
            .FirstOrDefault();

        if (!int.TryParse(maxAllowedBooksValue, out int maxAllowedBooks))
            throw new System.Exception("Maximum allowed books setting is missing or invalid.");

        int booksRented = dbContext.BookRequests
            .Count(r =>
                r.UserId == dto.UserId &&
                r.Status != RentStatusEnum.Returned &&
                r.RequestType == RequestTypeEnum.Rent
            );

        if (booksRented >= maxAllowedBooks)
            throw new System.Exception("You have reached the maximum allowed books limit.");
        
        var bookInventory = await dbContext.BookInventories
            .FirstOrDefaultAsync(x => x.BookId == dto.BookId);

        if (bookInventory == null)
            throw new System.Exception("Book not found");

        if (bookInventory.AvailableQuantity <= 0)
            throw new System.Exception("Not enough quantity available");

        bookInventory.AvailableQuantity--;

        var request = new BookRequest
        {
            BookId = dto.BookId,
            RequestType = dto.RequestType??RequestTypeEnum.Rent,
            UserId = dto.UserId
        };

        await dbContext.BookRequests.AddAsync(request);

        return await dbContext.SaveChangesAsync() > 0;
    }
}