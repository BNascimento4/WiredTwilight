namespace WiredTwilightBackend;

using System.ComponentModel.DataAnnotations;
using WiredTwilightBackend;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;

public class PostDTO
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Content { get; set; } = string.Empty;
}

