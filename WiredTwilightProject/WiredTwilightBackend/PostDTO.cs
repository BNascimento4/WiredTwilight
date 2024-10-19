namespace WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Para NotMapped
using BCrypt.Net; // Para BCrypt

public class PostDTO
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;  // Use string.Empty em vez de !default

    [Required]
    public string Content { get; set; } = string.Empty;  // Use string.Empty em vez de !default
}

