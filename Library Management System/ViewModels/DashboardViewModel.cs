namespace Library_Management_System.ViewModels;

public class DashboardViewModel
{
    public int TotalBooks { get; set; }
    public int AvailableBooks { get; set; }
    public int RentedBooks { get; set; }
    public int SoldBooks { get; set; }

    public List<RecentRentViewModel> RecentRents { get; set; } = new();
    public List<RecentSaleViewModel> RecentSales { get; set; } = new();
    public List<LostBookViewModel> LostBooks { get; set; } = new();
    public List<LowStockBookViewModel> LowStockBooks { get; set; } = new();
}

public class RecentRentViewModel
{
    public string BookName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime RentDate { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class RecentSaleViewModel
{
    public string BookName { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public DateTime SaleDate { get; set; }
    public decimal Amount { get; set; }
}

public class LostBookViewModel
{
    public string BookName { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public DateTime ReportedDate { get; set; }
}

public class LowStockBookViewModel
{
    public string BookName { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int AvailableQuantity { get; set; }
}
