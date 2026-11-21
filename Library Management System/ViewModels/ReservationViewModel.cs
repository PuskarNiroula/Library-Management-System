namespace Library_Management_System.ViewModels;

public class ReservationViewModel
{
    public int ReservationId { get; set; }
    public int BookId { get; set; }
    public string BookName { get; set; }
    public string? Isbn { get; set; }
    public string type {get; set;}
    public decimal Price { get; set; }
    
}