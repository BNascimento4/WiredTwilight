namespace WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;
public class Forum
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Title { get; set; }  // Nome do fórum

    [MaxLength(500)]
    public string Description { get; set; }  // Descrição do fórum

    public string CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }  // Criador do fórum

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<Post> Posts { get; set; }  // Postagens no fórum

    public bool IsActive { get; set; } = true;  // Se o fórum está ativo
}
