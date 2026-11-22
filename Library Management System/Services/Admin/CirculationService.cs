using Library_Management_System.Data;
using Library_Management_System.DTOs.Checkout;
using Library_Management_System.Enum;
using Library_Management_System.Models.DbModels;
using Library_Management_System.Services.Admin.Enum;
using Library_Management_System.Services.Admin.Exception;
using Library_Management_System.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services.Admin;

public class CirculationService(ApplicationDbContext context)
{
    public async Task CheckIn(CheckInRequestDto dto)
    {
        if (dto.Books == null || !dto.Books.Any())
            throw new System.Exception("No Books Provided");

        var rentLogIds = dto.Books
            .Select(b => b.RentLogId)
            .ToList();

        if (rentLogIds.Distinct().Count() != rentLogIds.Count)
            throw new System.Exception("Duplicate Rent Log IDs");

        await using var transaction =
            await context.Database.BeginTransactionAsync();

        try
        {
            var rentLogs = await context.RentLogs
                .Where(l => rentLogIds.Contains(l.RentLogId))
                .Where(l => l.StudentId == dto.StudentId)
                .Where(l => l.Status == RentLogStatusEnum.Active)
                .Include(rentLog => rentLog.Book)
                .ToListAsync();

            if (rentLogs.Count != rentLogIds.Count)
                throw new System.Exception("Some Active Rent Logs Were Not Found");

            var bookIds = rentLogs
                .Select(l => l.BookId)
                .Distinct()
                .ToList();

            var inventories = await context.BookInventories
                .Where(bi => bookIds.Contains(bi.BookId))
                .ToDictionaryAsync(bi => bi.BookId);

            if (inventories.Count != bookIds.Count)
                throw new System.Exception("Some Book Inventories Were Not Found");

            var checkInLookup = dto.Books.ToDictionary(b => b.RentLogId
            );

            foreach (var rentLog in rentLogs)
            {
                var inventory = inventories[rentLog.BookId];
                var checkIn = checkInLookup[rentLog.RentLogId];

                if (inventory.RentedQuantity <= 0)
                    throw new System.Exception("Invalid Rented Quantity");

                rentLog.Status = RentLogStatusEnum.Returned;

                inventory.RentedQuantity--;

                if (checkIn.IsLost)
                {
                    inventory.TotalQuantity--;

                    var lostRecord = new LostRecord
                    {
                        BookId = rentLog.BookId,
                        RentLogId = rentLog.RentLogId,
                        Price = rentLog.Book.Price,
                        StudentId = rentLog.StudentId,
                        RecordedOn = DateTime.Now
                    };
                    await context.LostRecords.AddAsync(lostRecord);
                }
                else
                {
                    inventory.AvailableQuantity++;
                }
            }

            await context.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
    
     public async Task CheckoutAsync(CheckoutDto dto, int approvedBy)
    {
        var maxAllowedBooksValue = await context.Settings
            .Where(s => s.Key == SettingKeyEnum.MaxBooksAllowed)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();

        if (!int.TryParse(maxAllowedBooksValue, out int maxAllowedBooks))
            throw new System.Exception("Maximum allowed books setting is missing or invalid.");

        var booksAlreadyRented = await context.BookRequests
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

        await using var transaction = await context.Database.BeginTransactionAsync();

        foreach (var book in dto.Books)
        {
            if (book.ReservationId != null)
            {
                var request = await context.BookRequests.FindAsync(book.ReservationId);

                if (request == null)
                    throw new BookRequestNotFoundException(book.ReservationId.Value);

                request.Status = BookRequestStatusEnum.Approved;
            }

            var inventory = await context.BookInventories
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

                await context.RentLogs.AddAsync(logs);
            }
            else if (book.TransactionType.ToUpper() == RequestTypeEnum.Sale.ToString().ToUpper())
            {
                var existingBook = await context.Books.FindAsync(book.BookId);

                if (existingBook == null)
                    throw new System.Exception($"Book {book.BookId} not found.");

                var sales = new SalesRecord
                {
                    BookId = book.BookId,
                    CustomerId = dto.StudentId,
                    Date = DateTime.Now,
                    Price = existingBook.Price
                };

                context.Add(sales);

                inventory.SoldQuantity++;
                inventory.TotalQuantity--;
            }

            await context.SaveChangesAsync();
        }

        await transaction.CommitAsync();
    }
     
     
    public async Task<List<CheckInResponseViewModel>> GetRentedBookLogs(int userId)
    {
        int maxDays = await GetMaxAllowedDays();
        int fineRate = await GetFineRate();
        DateTime today = DateTime.Today;

        var rentedBooks = await context.RentLogs.AsNoTracking()
            .Where(l => l.StudentId == userId && l.Status == RentLogStatusEnum.Active)
            .Select(a => new
            {
                a.RentLogId,
                a.BookId,
                BookName = a.Book.BookName,
                a.RentedDate,
                Price = a.Book.Price
            })
            .ToListAsync();

        return rentedBooks.Select(a =>
        {
            DateTime maxAllowedDate = a.RentedDate.AddDays(maxDays);

            return new CheckInResponseViewModel
            {
                RentedLogId = a.RentLogId,
                BookId = a.BookId,
                BookName = a.BookName,
                RentedDate = a.RentedDate,
                price = a.Price,
                MaxAllowedDate = maxAllowedDate,
                Fine = today > maxAllowedDate
                    ? fineRate * (today - maxAllowedDate).Days
                    : 0
            };
        }).ToList();
    }

    private async Task<int> GetFineRate()
    {
      var rate = await context.Settings.Where(s => s.Key == SettingKeyEnum.FineRate)
          .Select(s => s.Value)
          .FirstOrDefaultAsync();
      
      if (rate == null)
          throw new System.Exception("Fine rate is not set please set before checking in any student");
      
      return int.Parse(rate);
    }

    private async Task<int> GetMaxAllowedDays()
    {
        var allowedDays = await context.Settings.Where(s => s.Key == SettingKeyEnum.MaxAllowedDays)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();
        
        if (allowedDays==null)
            throw new System.Exception("Fine rate is not set please set before checking in any student");
        
        return int.Parse(allowedDays);
    }
}