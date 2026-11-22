using Library_Management_System.Models.DbModels;

namespace Library_Management_System.Models;

public class EditBookView
{
    public Book Book { get; set; }
    public List<Category> Categories { get; set; }
}