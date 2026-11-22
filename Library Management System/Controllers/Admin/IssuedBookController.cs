using Library_Management_System.Data;
using Library_Management_System.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers.Admin;
[Route("IssuedBooks")]
public class IssuedBookController(ApplicationDbContext dbContext):Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int rentPage = 1, int salesPage = 1,int lostRecordPage = 1, string tab = "rent")
    {
        const int pageSize = 10;

        var rentQuery = dbContext.RentLogs.AsNoTracking();

        var rentTotalRecords = await rentQuery.CountAsync();
        var rentTotalPages = (int)Math.Ceiling(rentTotalRecords / (double)pageSize);

        rentPage = Math.Clamp(rentPage, 1, Math.Max(1, rentTotalPages));

        var rents = await rentQuery
            .OrderBy(r => r.Status)
            .ThenByDescending(r => r.RentedDate)
            .Skip((rentPage - 1) * pageSize)
            .Take(pageSize)
            .Select(r => new RentBookViewModel
            {
                BookName = r.Book.BookName,
                StudentName = r.Student.FullName,
                RentedAt = r.RentedDate,
                Status = r.Status,
            }).ToListAsync();

        var salesQuery = dbContext.SalesRecords.AsNoTracking();

        var salesTotalRecords = await salesQuery.CountAsync();
        var salesTotalPages = (int)Math.Ceiling(salesTotalRecords / (double)pageSize);

        salesPage = Math.Clamp(salesPage, 1, Math.Max(1, salesTotalPages));

        var sales = await salesQuery
            .OrderByDescending(s => s.Date)
            .Skip((salesPage - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SalesBookViewModel
            {
                BookName = s.Book.BookName,
                StudentName = s.Customer.FullName,
                Price = s.Price,
                Date = s.Date
            })
            .ToListAsync();
        
        var lostRecordsQuery =  dbContext.LostRecords.AsNoTracking();

        var lostRecordTotalData = await lostRecordsQuery.CountAsync();
        var lostRecordTotalPages =(int)Math.Ceiling(lostRecordTotalData / (double)pageSize);
         lostRecordPage = Math.Clamp(lostRecordPage, 1, Math.Max(1, lostRecordTotalPages));
         var lostRecords = await lostRecordsQuery
             .Include(l => l.Student)
             .Include(l => l.Book)
             .OrderByDescending(lr => lr.RecordedOn)
             .Skip((lostRecordPage - 1) * pageSize)
             .Take(pageSize)
             .Select(lr => new LostRecordViewModel
             {
                 BookName = lr.Book.BookName,
                 Price = lr.Book.Price,
                 StudentName = lr.Student.FullName,
                 RecordedOn = lr.RecordedOn,
             }).ToListAsync();
        

        var vm = new IssuedBooksViewModel
        {
            RentBooks = rents,
            SalesBooks = sales,
            RentPage = rentPage,
            RentTotalPages = rentTotalPages,
            SalesPage = salesPage,
            SalesTotalPages = salesTotalPages,
            LostRecords = lostRecords,
            LostRecordPage = lostRecordPage,
            LostRecordTotalPages = lostRecordTotalPages,
            ActiveTab = tab
        };

        return View(vm);
    }
}
    
