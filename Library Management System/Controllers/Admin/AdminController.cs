using Library_Management_System.Data;
using Library_Management_System.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Controllers.Admin;

[Authorize(Roles = "Admin")]
public class AdminController(ApplicationDbContext context):Controller
{
    /// <summary>
    /// Displays the Admin area's main Index view.
    /// </summary>
    /// <returns>An <see cref="IActionResult"/> that renders the "Index" view.</returns>
    public async  Task<IActionResult> Index()
    {
        var totalBooks = context.BookInventories.Where(i => i.TotalQuantity > 0).Sum(i => i.TotalQuantity);
        var availableBooks = context.BookInventories.Where(i => i.AvailableQuantity > 0).Sum(i => i.AvailableQuantity);
        var rentedBooks = context.BookInventories.Where(i => i.RentedQuantity > 0).Sum(i => i.RentedQuantity);
        var soldBooks = context.BookInventories.Where(i => i.SoldQuantity > 0).Sum(i => i.SoldQuantity);



        var recentRents = await context.RentLogs.
            OrderByDescending(l => l.RentedDate)
            .Take(10)
            .Include(l=>l.Book)
            .Include(l=>l.Student)
            .Select(l =>
            new RecentRentViewModel
            {
                BookName = l.Book.BookName,
                StudentName = l.Student.FullName,
                RentDate = l.RentedDate,
                Status = l.Status
                
            }).ToListAsync();

        var recentSales = await context.SalesRecords.OrderByDescending(s => s.Date)
            .Take(10)
            .Include(s => s.Book)
            .Include(s => s.Customer)
            .Select(s => new RecentSaleViewModel
            {
                BookName = s.Book.BookName,
                CustomerName = s.Customer.FullName,
                SaleDate = s.Date,
                Amount = s.Price
            }).ToListAsync();

        var lostBooks = await context.LostRecords.OrderByDescending(lb => lb.BookId).Take(10)
            .Include(l => l.Book)
            .Include(l => l.Student)
            .Select(s => new LostBookViewModel
            {
                BookName = s.Book.BookName,
                StudentName = s.Student.FullName,
                ReportedDate = s.RecordedOn
            }).ToListAsync();
        
        var lowOnStock = await context.BookInventories.Where(b => b.AvailableQuantity < 11)
            .Include(l => l.Book)
            .Select(b=> new LowStockBookViewModel
            {
                BookName = b.Book.BookName,
                Author = b.Book.Author,
                AvailableQuantity = b.AvailableQuantity,
            }).ToListAsync();
            
        
        
        var vm = new DashboardViewModel
        {
            TotalBooks = totalBooks,
            AvailableBooks = availableBooks,
            RentedBooks = rentedBooks,
            SoldBooks = soldBooks,
            RecentRents = recentRents,
            RecentSales = recentSales,
            LostBooks = lostBooks,
            LowStockBooks = lowOnStock
        };
        
        
        
        return View("Index",vm);
    }
    
}