namespace Library_Management_System.ViewModels;

public class BookRentalViewModel
{
    public string BookName { get; set; }
    public string AuthorName { get; set; }
    public string BookImageUrl { get; set; }
    public decimal Price { get; set; }
    public DateTime RentedDate { get; set; }
    public DateTime DeadlineDate { get; set; }
    public decimal Fine { get; set; }
}