namespace WiredTwilightBackend;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class Comment
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string Content { get; set; }  // Conteúdo do comentário

    public string CreatedByUserId { get; set; }
    public User CreatedByUser { get; set; }  // Usuário que comentou

    public int PostId { get; set; }
    public Post Post { get; set; }  // Post ao qual o comentário pertence

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
