using Library_Management_System.Models.DbModels;

namespace Library_Management_System.ViewModels;

public class CategoriesViewModel
{
    public IEnumerable<Category> Categories { get; set; } = new List<Category>();
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}