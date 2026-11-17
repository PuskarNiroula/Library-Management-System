using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Management_System.Models;

public class RentLog
{
    [Key]
    public int RentLogId { get; set; }
    
    [Required]
    public int IncomeId { get; set; }
    
    [Required]
    public string Status { get; set; }
    
    [ForeignKey("IncomeId")]
    public Income Income { get; set; }
    
}