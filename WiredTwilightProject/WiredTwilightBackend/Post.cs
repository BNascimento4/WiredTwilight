namespace WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;
public class Post
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Title { get; set; }  // Título do post

    [Required]
    public string Content { get; set; }  // Conteúdo do post


    public string? CreatedByUserId { get; set; }

    public User CreatedByUser { get; set; }  // Usuário que criou o post

    public int ForumId { get; set; }
    public Forum Forum { get; set; }  // Fórum ao qual o post pertence

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Comment> Comments { get; set; }  // Comentários no post

    public ICollection<Tag> Tags { get; set; }  // Tags associadas ao post
    public Post()
    {
        CreatedByUserId = "default"; // Initialize with a default value
    }
}
