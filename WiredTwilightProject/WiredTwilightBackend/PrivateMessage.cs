namespace WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Para NotMapped
using BCrypt.Net; // Para BCrypt
public class PrivateMessage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }  // Conteúdo da mensagem

    [Required]
    public string FromUserId { get; set; } = string.Empty;  // Inicializado como string vazia

    public User FromUser { get; set; }  // Remetente

    [Required]
    public string ToUserId { get; set; } = string.Empty;  // Inicializado como string vazia

    public User ToUser { get; set; }  // Destinatário

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
