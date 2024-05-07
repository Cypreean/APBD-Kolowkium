using System.ComponentModel.DataAnnotations;

namespace Kolokwium_APBD.Models;

public class BooksAuthors
{
    [Required]
    public int IdBook { get; set; }
    [MaxLength(100)]
    public string Title { get; set; }
    public List<Authors> Authors { get; set; }
}
public class BooksAuthorsNoId
{
    [Required]
    [MaxLength(100)]
    public string Title { get; set; }
    public List<Authors> Authors { get; set; }
}