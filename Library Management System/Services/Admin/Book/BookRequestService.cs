using Library_Management_System.Data;
using Library_Management_System.Models;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Services.Admin.Book;

public class BookRequestService(ApplicationDbContext dbContext) :IBookRequestService
{
    private readonly ApplicationDbContext _context= dbContext;
    public async Task<bool> CreateBookRequestAsync(BookRequest bookRequest)
    {
        _context.BookRequests.Add(bookRequest);
        return await _context.SaveChangesAsync() > 0;
    }
}