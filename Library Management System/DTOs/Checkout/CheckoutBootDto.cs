namespace Library_Management_System.DTOs.Checkout;

public class CheckoutBookDto
{
    public int BookId { get; set; }
    public string TransactionType { get; set; } = string.Empty;
    public int? ReservationId { get; set; }
}