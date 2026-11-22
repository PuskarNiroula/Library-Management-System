
using Library_Management_System.Data;
using Library_Management_System.DTOs.Checkout;
using Library_Management_System.Enum;
using Library_Management_System.Models;
using Library_Management_System.Models.DbModels;
using Library_Management_System.Services.Admin.Enum;
using Library_Management_System.Services.Admin.Exception;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services.Admin.BookRequest;

public class BookRequestService(ApplicationDbContext dbContext) : IBookRequestService
{
    public async Task Reject(int id)
    {
        var request = await dbContext.BookRequests.FindAsync(id);

        if (request == null)
            throw new BookRequestNotFoundException(id);

        request.Status = BookRequestStatusEnum.Cancelled;

        await dbContext.SaveChangesAsync();
    }

    public async Task Approve(int id, int userId)
    {
        CheckForMaxAllowedBooks(userId);

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var request = await dbContext.BookRequests.FindAsync(id);

        if (request == null)
            throw new BookRequestNotFoundException(id);

        if (request.Status != BookRequestStatusEnum.Pending)
            throw new System.Exception("Request is already " + request.Status);

        var bookStock = await dbContext.BookInventories.FindAsync(request.BookId);

        if (bookStock == null)
            throw new BookInventoryRecordNotFound(request.BookId);

        if (bookStock.AvailableQuantity <= 0)
            throw new BookNotAvailableException();

        bookStock.AvailableQuantity--;

        if (request.RequestType == RequestTypeEnum.Rent)
        {
            bookStock.RentedQuantity++;
        }
        else if (request.RequestType == RequestTypeEnum.Sale)
        {
            bookStock.SoldQuantity++;
        }

        request.Status = BookRequestStatusEnum.Reserved;

        await dbContext.SaveChangesAsync();

        await transaction.CommitAsync();
    }

   

    private void CheckForMaxAllowedBooks(int userId, bool isCheckout = false)
    {
        var maxAllowedBooksValue = dbContext.Settings
            .Where(s => s.Key == SettingKeyEnum.MaxBooksAllowed)
            .Select(s => s.Value)
            .FirstOrDefault();

        if (!int.TryParse(maxAllowedBooksValue, out int maxAllowedBooks))
            throw new System.Exception("Maximum allowed books setting is missing or invalid.");

        var booksRented = dbContext.BookRequests
            .Count(r =>
                r.UserId == userId &&
                r.Status != RentStatusEnum.Returned &&
                r.RequestType == RequestTypeEnum.Rent
            );

        if (booksRented >= maxAllowedBooks)
            throw new System.Exception("You have reached the maximum allowed books limit.");
    }
}
