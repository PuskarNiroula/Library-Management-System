using Library_Management_System.DTOs.Book;
using Library_Management_System.Models;

namespace Library_Management_System.Services.Admin.Book;

public interface IBookService
{
    Task<bool> CreateBookAsync(CreateBookDto createBookDto);
    Task<bool> UpdateBookAsync(UpdateBookDto updateBookDto);
    Task<bool> DeleteBookAsync(int id);

    Task<bool> SaleOrRentBook(int id,string type);
    Task<Models.Book> GetBookAsync(int id);
    
    Task<List<Models.Book>> GetNewBooks();
    Task<PaginatedBook<Models.Book>> GetSearchedBook(string searchString,int page=1,int pageSize=6);
    
    Task<PaginatedBook<Models.Book>> GetPaginatedBooks(int page=1,int pageSize=6);
    
    Task<Models.Book> GetBookByIdAsync(int id);

}