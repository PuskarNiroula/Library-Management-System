namespace Library_Management_System.ViewModels;
public class CheckInResponseViewModel
{
    public int RentedLogId { get; set; }
    public int BookId { get; set; }
    public string BookName { get; set; }
    public DateTime RentedDate { get; set; }
    public decimal price { get; set; }
    public DateTime MaxAllowedDate { get; set; }
    public decimal Fine { get; set; }
}