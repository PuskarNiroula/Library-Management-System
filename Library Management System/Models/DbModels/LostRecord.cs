using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Management_System.Models.DbModels;


public class LostRecord
{
    [Key]
    public int LostRecordId { get; set; }
    
    [Required]
    public int BookId { get; set; }
    
    [Required]
    public float Price { get; set; }
    
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