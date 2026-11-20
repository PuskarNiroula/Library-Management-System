
namespace Library_Management_System.DTOs.Book;

public class CreateBookDto
{
    public string BookName { get; set; }
    public string Author { get; set; }
    public string Isbn { get; set; }
    public float Price { get; set; }
    public int? Quantity { get; set; } = null;
    public DateOnly PublishDate { get; set; } 
    public int CategoryId { get; set; }
    public string Publisher { get; set; }
    
    public IFormFile? Image { get; set; }
    
    public string? ImageUrl { get; set; }
}