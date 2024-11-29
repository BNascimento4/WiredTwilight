namespace WiredTwilightBackend;

using WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;
public class PrivateMessage
{
    [Key]
    public int Id { get; set; }

    [Required]
    public string? Content { get; set; }

    [Required]
    public string? FromUserId { get; set; } = string.Empty;

    public User FromUser { get; set; }

    [Required]
    public string ToUserId { get; set; } = string.Empty;

    public User ToUser { get; set; }

    public DateTime SentAt { get; set; } = DateTime.UtcNow;
}
