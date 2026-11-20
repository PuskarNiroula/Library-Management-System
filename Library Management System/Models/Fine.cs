using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Management_System.Models;

public class Fine
{
    [Key]
    public int FineId { get; set; }

    [Required]
    public int RentLogId { get; set; }

    [ForeignKey(nameof(RentLogId))]
    public RentLog RentLog { get; set; }

    [Required]
    public decimal Amount { get; set; }

    [Required]
    public int LateDays { get; set; }

    [Required]
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public bool IsPaid { get; set; } = false;

    public DateTime? PaidDate { get; set; }
}