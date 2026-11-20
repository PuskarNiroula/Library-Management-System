using Library_Management_System.Data;
using Library_Management_System.Models;
using Library_Management_System.Services.Admin.Enum;
using Library_Management_System.Services.Admin.Exception;


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
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var request = await dbContext.BookRequests.FindAsync(id);

        if (request == null)
            throw new BookRequestNotFoundException(id);

        if (request.Status == BookRequestStatusEnum.Approved)
            throw new System.Exception("Request is already approved.");

        var bookStock = await dbContext.BookInventories.FindAsync(request.BookId);

        if (bookStock == null)
            throw new BookInventoryRecordNotFound(request.BookId);

        if (bookStock.AvailableQuantity <= 0)
            throw new BookNotAvailableException();

        bookStock.AvailableQuantity--;

        if (request.RequestType == RequestTypeEnum.Rent)
        {
            bookStock.RentedQuantity++;

            var rentLog = new RentLog
            {
                BookId = request.BookId,
                ApprovedById = userId,
                StudentId = request.UserId,
                Status = RentStatusEnum.Active,
                RentedDate = DateTime.UtcNow
            };

            await dbContext.RentLogs.AddAsync(rentLog);
        }
        else if (request.RequestType == RequestTypeEnum.Sale)
        {
            bookStock.SoldQuantity++;
        }

        request.Status = BookRequestStatusEnum.Approved;

        await dbContext.SaveChangesAsync();

        await transaction.CommitAsync();
    }
}