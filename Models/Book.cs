using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Book
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required(ErrorMessage = "Title is required.")]
    [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Author is required.")]
    [StringLength(50, ErrorMessage = "Author cannot be longer than 50 characters.")]
    public string Author { get; set; }

    [Required(ErrorMessage = "Language is required.")]
    [StringLength(20, ErrorMessage = "Language cannot be longer than 20 characters.")]
    public string Language { get; set; }

    [Required(ErrorMessage = "Category is required.")]
    [StringLength(50, ErrorMessage = "Category cannot be longer than 50 characters.")]
    public string Category { get; set; }

    [Range(1450, 2100, ErrorMessage = "Published Year must be between 1450 and 2100.")]
    public int PublishedYear { get; set; }
}