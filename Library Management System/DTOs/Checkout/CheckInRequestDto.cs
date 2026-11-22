namespace Library_Management_System.DTOs.Checkout;

public class CheckInRequestDto
{
    public int StudentId { get; set; }
    public DateTime CheckinDate { get; set; }
    public List<CheckInBookDto> Books { get; set; } = new();
}

public class CheckInBookDto
{
    public int RentLogId { get; set; }
    public bool IsLost { get; set; }
}