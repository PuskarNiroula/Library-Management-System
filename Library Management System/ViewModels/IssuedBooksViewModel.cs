
using Library_Management_System.Models.DbModels;

namespace Library_Management_System.ViewModels;

public class IssuedBooksViewModel
{
    public IEnumerable<RentBookViewModel> RentBooks { get; set; } = [];
    public IEnumerable<SalesBookViewModel> SalesBooks { get; set; } = [];
    public IEnumerable<LostRecordViewModel> LostRecords { get; set; } = [];

    public int RentPage { get; set; }
    public int RentTotalPages { get; set; }

    public int SalesPage { get; set; }
    public int SalesTotalPages { get; set; }
    
    public int LostRecordPage { get; set; }
    public int LostRecordTotalPages { get; set; }

    public string ActiveTab { get; set; } = "rent";
}

