using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library_Management_System.Models;

public partial class Book
{
    [Key]
    public int BookId { get; set; }

    [Required]
    public string BookName { get; set; }

    [Required]
    public string Author { get; set; }

    [Required]
    public string Publisher { get; set; }

    [Required]
    public string Isbn { get; set; }

    public string? ImageUrl { get; set; }

    [Required]
    public int CategoryId { get; set; }

    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }

    [Required]
    public decimal Price { get; set; }

    public DateOnly? PublicationDate { get; set; }

    [Required]
    public int Quantity { get; set; }
}


