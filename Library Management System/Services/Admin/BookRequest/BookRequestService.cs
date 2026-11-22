
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

    public async Task CheckoutAsync(CheckoutDto dto, int approvedBy)
    {
        var maxAllowedBooksValue = await dbContext.Settings
            .Where(s => s.Key == SettingKeyEnum.MaxBooksAllowed)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();

        if (!int.TryParse(maxAllowedBooksValue, out int maxAllowedBooks))
            throw new System.Exception("Maximum allowed books setting is missing or invalid.");

        var booksAlreadyRented = await dbContext.BookRequests
            .CountAsync(r =>
                r.UserId == dto.StudentId &&
                r.Status == BookRequestStatusEnum.Approved &&
                r.RequestType == RequestTypeEnum.Rent
            );

        var newRentalBooks = dto.Books.Count(b =>
            b.TransactionType.ToUpper() == RequestTypeEnum.Rent.ToString().ToUpper()
        );

        if (booksAlreadyRented + newRentalBooks > maxAllowedBooks)
            throw new System.Exception(
                $"You cannot rent {newRentalBooks} books. You already have {booksAlreadyRented} rented books and your maximum allowed limit is {maxAllowedBooks}."
            );

        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        foreach (var book in dto.Books)
        {
            if (book.ReservationId != null)
            {
                var request = await dbContext.BookRequests.FindAsync(book.ReservationId);

                if (request == null)
                    throw new BookRequestNotFoundException(book.ReservationId.Value);

                request.Status = BookRequestStatusEnum.Approved;
            }

            var inventory = await dbContext.BookInventories
                .Where(b => b.BookId == book.BookId)
                .FirstOrDefaultAsync();

            if (inventory == null)
                throw new BookInventoryRecordNotFound(book.BookId);

            if (inventory.AvailableQuantity <= 0)
                throw new BookNotAvailableException();

            inventory.AvailableQuantity--;

            if (book.TransactionType.ToUpper() == RequestTypeEnum.Rent.ToString().ToUpper())
            {
                inventory.RentedQuantity++;

                var logs = new RentLog
                {
                    BookId = book.BookId,
                    StudentId = dto.StudentId,
                    ApprovedById = approvedBy,
                    Status = RentStatusEnum.Active,
                    RentedDate = DateTime.Now
                };

                await dbContext.RentLogs.AddAsync(logs);
            }
            else if (book.TransactionType.ToUpper() == RequestTypeEnum.Sale.ToString().ToUpper())
            {
                var existingBook = await dbContext.Books.FindAsync(book.BookId);

                if (existingBook == null)
                    throw new System.Exception($"Book {book.BookId} not found.");

                var sales = new SalesRecord
                {
                    BookId = book.BookId,
                    CustomerId = dto.StudentId,
                    Date = DateTime.Now,
                    Price = existingBook.Price
                };

                dbContext.Add(sales);

                inventory.SoldQuantity++;
                inventory.TotalQuantity--;
            }

            await dbContext.SaveChangesAsync();
        }

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
