namespace WiredTwilightBackend;

using System.ComponentModel.DataAnnotations;
using WiredTwilightBackend;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;



public class Forum
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public required string? Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; }

    public string CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public bool IsActive { get; set; } = true;
    public Forum()
    {

    }
}

