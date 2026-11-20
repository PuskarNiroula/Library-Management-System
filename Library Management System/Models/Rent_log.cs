using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Management_System.Models;

public class RentLog
{
    [Key]
    public int RentLogId { get; private set; }

    [Required]
    public int BookId { get; set; }

    [ForeignKey(nameof(BookId))]
    public Book Book { get; set; }

    [Required]
    public int StudentId { get; set; }

    [ForeignKey(nameof(StudentId))]
    public User Student { get; set; }

    [Required]
    public int ApprovedById { get; set; }

    [ForeignKey(nameof(ApprovedById))]
    public User ApprovedBy { get; set; }

    [Required]
    public string Status { get; set; }

    public DateTime RentedDate { get; set; } = DateTime.UtcNow;

    public DateTime? ReturnedDate { get; set; }
}