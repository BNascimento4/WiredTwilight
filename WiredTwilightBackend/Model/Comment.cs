namespace WiredTwilightBackend;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System;

public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(1000)] // Limite de caracteres para o conteúdo do comentário
    public string Content { get; set; } // Propriedade não nula

    public string CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }

    public int PostId { get; set; }
    public Post Post { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Construtor padrão
    public Comment() { }

    // Construtor opcional para inicialização rápida
    public Comment(string content, string createdByUserId)
    {
        Content = content;
        CreatedByUserId = createdByUserId;
    }
}
