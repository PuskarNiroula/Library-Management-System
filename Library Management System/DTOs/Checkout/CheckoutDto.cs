namespace Library_Management_System.DTOs.Checkout;

public class CheckoutDto
{
    public int StudentId { get; set; }
    public DateOnly CheckoutDate { get; set; }
    public List<CheckoutBookDto> Books { get; set; } = new();
    
}