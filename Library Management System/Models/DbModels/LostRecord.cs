using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Library_Management_System.Models.DbModels;

[Index(nameof(RentLogId), IsUnique = true)]
public class LostRecord
{
    [Key]
    public int LostRecordId { get; set; }
    
    [Required]
    public int BookId { get; set; }
    
    [Required]
    [Column(TypeName = "decimal(18, 2)")]
    public decimal Price { get; set; }
    
    [Required]
    public int StudentId { get; set; }
    
    [Required]
    public DateTime RecordedOn { get; set; }
    
    [Required]
    public int RentLogId { get; set; }
    
    
    [ForeignKey(nameof(StudentId))]
    public User Student { get; set; }
    
    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; }
    
    [ForeignKey(nameof(RentLogId))]
    public RentLog RentLog { get; set; }
}