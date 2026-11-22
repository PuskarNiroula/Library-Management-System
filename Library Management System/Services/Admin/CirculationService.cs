using Library_Management_System.Data;
using Library_Management_System.DTOs.Checkout;
using Library_Management_System.Enum;
using Library_Management_System.Models.DbModels;
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

        var checkInLookup = dto.Books.ToDictionary(
            b => b.RentLogId
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

    
}