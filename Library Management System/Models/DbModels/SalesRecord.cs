using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Management_System.Models.DbModels;

public class SalesRecord
{
    [Key]
    public int SalesRecordId { get; set; }
    [Required]
    public int BookId { get; set; }
    [Required]
    public int CustomerId { get; set; }
    [Required]
    [Column(TypeName = "decimal(18,2)")]
    public decimal Price { get; set; }
    [Required]
    public DateTime Date { get; set; }
    
    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; }
    
    [ForeignKey(nameof(CustomerId))]
    public User Customer { get; set; }
    
}