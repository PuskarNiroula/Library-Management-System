
using System.Data;
using Library_Management_System.Data;
using Library_Management_System.DTOs.Book;
using Library_Management_System.Enum;
using Library_Management_System.Models.DbModels;
using Library_Management_System.Services.Admin.Enum;
using Library_Management_System.Services.Student.Exception;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services.Student;

public class BookInventoryService(ApplicationDbContext dbContext)
    : IBookInventoryService
{
    public async Task<bool> RequestBookAsync(BookRequestDto dto)
    {
        var requestType = dto.RequestType ?? RequestTypeEnum.Rent;

        await using var transaction =
            await dbContext.Database.BeginTransactionAsync(
                IsolationLevel.Serializable);

        await ValidateInventory(dto.BookId);
        
        if (requestType == RequestTypeEnum.Rent)
        {
            await ValidateForDuplicateRequest(dto.UserId, dto.BookId);
            await ValidateForMaxAllowedBooks(dto.UserId);
            
        }

        var request = new BookRequest
        {
            BookId = dto.BookId,
            RequestType = requestType,
            UserId = dto.UserId
        };

        dbContext.BookRequests.Add(request);

        await dbContext.SaveChangesAsync();

        await transaction.CommitAsync();

        return true;
    }

    private async Task ValidateForDuplicateRequest(int userId, int bookId)
    {
        var hasExistingRequest = await dbContext.BookRequests
            .AnyAsync(r =>
                r.UserId == userId &&
                r.BookId == bookId &&
                r.RequestType == RequestTypeEnum.Rent &&
                r.Status != BookRequestStatusEnum.Returned &&
                r.Status != BookRequestStatusEnum.Cancelled);

        if (hasExistingRequest)
            throw new DuplicateBookRequestException();

        var hasActiveRental = await dbContext.RentLogs
            .AnyAsync(r =>
                r.StudentId == userId &&
                r.BookId == bookId &&
                r.Status == RentLogStatusEnum.Active);

        if (hasActiveRental)
            throw new DuplicateBookRequestException();
    }

    private async Task ValidateForMaxAllowedBooks(int userId)
    {
        var settingValue = await dbContext.Settings
            .Where(s => s.Key == SettingKeyEnum.MaxBooksAllowed)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();

        if (!int.TryParse(settingValue, out var maxAllowedBooks) ||
            maxAllowedBooks <= 0)
        {
            throw new InvalidOperationException(
                "Maximum allowed books setting is missing or invalid.");
        }

        var booksCount = await dbContext.BookRequests
            .CountAsync(r =>
                r.UserId == userId &&
                r.RequestType == RequestTypeEnum.Rent &&
                r.Status != BookRequestStatusEnum.Returned &&
                r.Status != BookRequestStatusEnum.Cancelled);

        if (booksCount >= maxAllowedBooks)
        {
            throw new InvalidOperationException(
                "You have reached the maximum allowed books limit.");
        }
    }

    private async Task ValidateInventory(int bookId)
    {
        var inventory = await dbContext.BookInventories
            .FirstOrDefaultAsync(bi => bi.BookId == bookId);

        if (inventory == null)
            throw new InvalidOperationException("Book inventory not found.");

        if (inventory.AvailableQuantity <= 0)
            throw new InvalidOperationException(
                "Not enough quantity available.");
        
    }
}
