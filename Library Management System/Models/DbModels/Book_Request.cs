using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Management_System.Models;

public class BookRequest
{
    [Key]
    public int BookRequestId { get; set; }
    [Required]
    public string RequestType { get; set; }
    [Required]
    public int UserId { get; set; }
    [Required]
    public int BookId { get; set; }

    public string Status { get; set; }="pending";

    [ForeignKey("BookId")]
   public Book Book { get; set; }
   [ForeignKey("UserId")]
   public User User{ get; set; }
   
   
}