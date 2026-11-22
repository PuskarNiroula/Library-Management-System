using System.ComponentModel.DataAnnotations;

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
}