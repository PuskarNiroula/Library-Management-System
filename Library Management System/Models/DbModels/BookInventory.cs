using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Library_Management_System.Models.DbModels;

namespace Library_Management_System.Models.DbModels;

public class BookInventory
{
    [Key]
    public int BookInventoryId { get; set; }

    [Required]
    public int BookId { get; set; }

    [Required]
    public int TotalQuantity { get; set; }

    [Required]
    public int AvailableQuantity { get; set; }

    [Required]
    public int SoldQuantity { get; set; }

    [Required]
    public int RentedQuantity { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; }
}