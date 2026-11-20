using Library_Management_System.DTOs.Book;

namespace Library_Management_System.Services.Student;

public interface IBookInventoryService
{
    public Task<bool> RequestBookAsync(BookRequestDto dto);
}