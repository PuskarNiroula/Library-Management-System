using Library_Management_System.Models;

namespace Library_Management_System.Services.Admin.Book;

public interface IBookRequestService
{
    
    public Task<bool> CreateBookRequestAsync(BookRequest bookRequest);
    
}