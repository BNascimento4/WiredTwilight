namespace WiredTwilightBackend;

using WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;

public class Tag
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string? Name { get; set; }

    public ICollection<Post>? Posts { get; set; }
}
