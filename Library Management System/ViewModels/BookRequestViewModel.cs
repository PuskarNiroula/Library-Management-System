namespace Library_Management_System.ViewModels;

public class BookRequestViewModel
{
    public int BookRequestId { get; set; }

    public string RequestType { get; set; }

    public string? StudentName { get; set; }

    public string BookName { get; set; }

    public string? BookImageUrl { get; set; }

    public string Status { get; set; }
}