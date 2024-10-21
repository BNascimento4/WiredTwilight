namespace WiredTwilightBackend;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;
using WiredTwilightBackend;

public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }

    [Required]
    public string Content { get; set; }

    public string? CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; }
    public int ForumId { get; set; }
    public Forum Forum { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    public Post()
    {
        CreatedByUserId = "default";
    }
}
