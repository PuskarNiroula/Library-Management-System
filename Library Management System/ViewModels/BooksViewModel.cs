using Library_Management_System.Models.DbModels;

namespace Library_Management_System.ViewModels;

public class BooksViewModel
{
    public IEnumerable<Book> Books { get; set; } = new List<Book>();

    public int CurrentPage { get; set; } = 1;

    public int TotalPages { get; set; }
}
