
namespace Library_Management_System.ViewModels;

public class IssuedBooksViewModel
{
    public IEnumerable<RentBookViewModel> RentBooks { get; set; } = [];
    public IEnumerable<SalesBookViewModel> SalesBooks { get; set; } = [];

    public int RentPage { get; set; }
    public int RentTotalPages { get; set; }

    public int SalesPage { get; set; }
    public int SalesTotalPages { get; set; }

    public string ActiveTab { get; set; } = "rent";
}

