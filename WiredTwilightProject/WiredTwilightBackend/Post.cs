namespace WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Para NotMapped
using BCrypt.Net; // Para BCrypt
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

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();  // Inicializado aqui

    public ICollection<Tag> Tags { get; set; } = new List<Tag>();  // Inicializado aqui

    public Post()
    {
        CreatedByUserId = "default"; // Inicialização padrão, se necessário
    }
}
