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
                rentLog.ReturnedDate = DateTime.Now;

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
        await ValidateRentalLimitAsync(dto);

        await using var transaction = await context.Database.BeginTransactionAsync();

        foreach (var book in dto.Books)
        {
            await ProcessCheckoutBookAsync(book, dto.StudentId, approvedBy);
        }

        await context.SaveChangesAsync();
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

    public async Task<List<BookRentalViewModel>> GetRentedLogs(int userId)
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
                AuthorName= a.Book.Author,
                a.RentedDate,
                Price = a.Book.Price
            })
            .ToListAsync();

        return rentedBooks.Select(a =>
        {
            DateTime maxAllowedDate = a.RentedDate.AddDays(maxDays);

            return new BookRentalViewModel
            {
                
                BookName = a.BookName,
                AuthorName = a.AuthorName,
                RentedDate = a.RentedDate,
                Price = a.Price,
                DeadlineDate = maxAllowedDate,
                Fine = today > maxAllowedDate
                    ? fineRate * (today - maxAllowedDate).Days
                    : 0
            };
        }).ToList();
        
    }

    private async Task ValidateRentalLimitAsync(CheckoutDto dto)
    {
        var maxAllowedBooksValue = await context.Settings
            .Where(s => s.Key == SettingKeyEnum.MaxBooksAllowed)
            .Select(s => s.Value)
            .FirstOrDefaultAsync();

        if (!int.TryParse(maxAllowedBooksValue, out var maxAllowedBooks))
            throw new System.Exception("Maximum allowed books setting is missing or invalid.");

        var booksAlreadyRented = await context.BookRequests
            .CountAsync(r =>
                r.UserId == dto.StudentId &&
                r.Status == BookRequestStatusEnum.Approved &&
                r.RequestType == RequestTypeEnum.Rent
            );

        var newRentalBooks = dto.Books.Count(b =>
            string.Equals(
                b.TransactionType,
                RequestTypeEnum.Rent,
                StringComparison.OrdinalIgnoreCase
            )
        );

        if (booksAlreadyRented + newRentalBooks > maxAllowedBooks)
        {
            throw new System.Exception(
                $"You cannot rent {newRentalBooks} books. You already have {booksAlreadyRented} rented books and your maximum allowed limit is {maxAllowedBooks}."
            );
        }
    }

    private async Task ProcessCheckoutBookAsync(
        CheckoutBookDto book,
        int studentId,
        int approvedBy)
    {
        var isReserved = book.ReservationId.HasValue;

        ValidateTransactionType(book.TransactionType);

        if (isReserved)
        {
            await ApproveReservationAsync(book.ReservationId.Value);
        }

        BookInventory? inventory = null;

        if (!isReserved)
        {
            inventory = await GetAvailableInventoryAsync(book.BookId);
            inventory.AvailableQuantity--;
        }

        if (string.Equals(
                book.TransactionType,
                RequestTypeEnum.Rent,
                StringComparison.OrdinalIgnoreCase))
        {
            await ProcessRentAsync(
                book.BookId,
                studentId,
                approvedBy,
                inventory
            );
        }
        else
        {
            await ProcessSaleAsync(
                book.BookId,
                studentId,
                inventory
            );
        }
    }

    private void ValidateTransactionType(string transactionType)
    {
        if (!string.Equals(
                transactionType,
                RequestTypeEnum.Rent,
                StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(
                transactionType,
                RequestTypeEnum.Sale,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new System.Exception($"Invalid transaction type: {transactionType}");
        }
    }

    private async Task ApproveReservationAsync(int reservationId)
    {
        var request = await context.BookRequests.FindAsync(reservationId);

        if (request == null)
            throw new BookRequestNotFoundException(reservationId);

        request.Status = BookRequestStatusEnum.Approved;
    }

    private async Task<BookInventory> GetAvailableInventoryAsync(int bookId)
    {
        var inventory = await context.BookInventories
            .FirstOrDefaultAsync(b => b.BookId == bookId);

        if (inventory == null)
            throw new BookInventoryRecordNotFound(bookId);

        if (inventory.AvailableQuantity <= 0)
            throw new BookNotAvailableException();

        return inventory;
    }

    private async Task ProcessRentAsync(
        int bookId,
        int studentId,
        int approvedBy,
        BookInventory? inventory)
    {
        if (inventory != null)
            inventory.RentedQuantity++;

        var rentLog = new RentLog
        {
            BookId = bookId,
            StudentId = studentId,
            ApprovedById = approvedBy,
            Status = RentStatusEnum.Active,
            RentedDate = DateTime.Now
        };

        await context.RentLogs.AddAsync(rentLog);
    }

    private async Task ProcessSaleAsync(
        int bookId,
        int studentId,
        BookInventory? inventory)
    {
        var existingBook = await context.Books.FindAsync(bookId);

        if (existingBook == null)
            throw new System.Exception($"Book {bookId} not found.");

        var sales = new SalesRecord
        {
            BookId = bookId,
            CustomerId = studentId,
            Date = DateTime.Now,
            Price = existingBook.Price
        };

        await context.SalesRecords.AddAsync(sales);

        if (inventory != null)
        {
            inventory.SoldQuantity++;
            inventory.TotalQuantity--;
        }
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

        if (allowedDays == null)
            throw new System.Exception("Fine rate is not set please set before checking in any student");

        return int.Parse(allowedDays);
    }
}