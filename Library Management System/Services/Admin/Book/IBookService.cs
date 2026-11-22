using Library_Management_System.DTOs;
using Library_Management_System.DTOs.Book;
using Library_Management_System.Models;

namespace Library_Management_System.Services.Admin.Book;

public interface IBookService
{
    Task<bool> CreateBookAsync(CreateBookDto createBookDto);
    Task<bool> UpdateBookAsync(UpdateBookDto updateBookDto);
    Task<bool> DeleteBookAsync(int id);
    Task<Models.DbModels.Book> GetBookAsync(int id);
    
    Task<List<Models.DbModels.Book>> GetNewBooks();
    Task<PaginatedBook<Models.DbModels.Book>> GetSearchedBook(string searchString,int page=1,int pageSize=6);
    
    Task<PaginatedBook<Models.DbModels.Book>> GetPaginatedBooks(int page=1,int pageSize=6);

    Task AddBookQuantityAsync(AddBookRequestDto dto);

}