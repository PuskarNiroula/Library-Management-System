using Library_Management_System.Data;
using Library_Management_System.DTOs.Checkout;
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
        await using var transaction = await dbContext.Database.BeginTransactionAsync();

        var request = await dbContext.BookRequests.FindAsync(id);

        if (request == null)
            throw new BookRequestNotFoundException(id);

        if (request.Status != BookRequestStatusEnum.Pending)
            throw new System.Exception("Request is already "+request.Status);

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

    public async Task CheckoutAsync(CheckoutDto dto,int approvedBy)
    {
        foreach (var book in dto.Books)
        {
            if (book.ReservationId != null)
            {
                var request = await dbContext.BookRequests.FindAsync(book.ReservationId);
                if (request == null)
                    throw new BookRequestNotFoundException(book.ReservationId ?? 0);
                request.Status = BookRequestStatusEnum.Approved;
            }

            var inventory = await dbContext.BookInventories.Where(b => b.BookId == book.BookId).FirstOrDefaultAsync();
              
              if (inventory == null)
                  throw new BookInventoryRecordNotFound(book.BookId);
              
              inventory.AvailableQuantity--;

              if (book.TransactionType.ToUpper()== RequestTypeEnum.Rent)
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
              else if (book.TransactionType.ToUpper() == RequestTypeEnum.Sale)
              {


                  var existingBook = await dbContext.Books
                      .FindAsync(book.BookId);

                  if (existingBook == null)
                      throw new System.Exception($"Book {book.BookId} not found.");
                  var sales = new SalesRecord
                  {
                      BookId = book.BookId,
                      CustomerId = dto.StudentId,
                      Date = DateTime.Now,
                      Price =existingBook.Price,
                  };
                  dbContext.Add(sales);
                  
                  inventory.SoldQuantity++;
                  inventory.TotalQuantity--;
              }

            
              await dbContext.SaveChangesAsync();
        }
            
            
        
        
        
    
        
        
        
        
    }
}