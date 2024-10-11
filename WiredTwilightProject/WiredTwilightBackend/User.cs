namespace WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Para NotMapped
using BCrypt.Net; // Para BCrypt

public class User
{
    [Key]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    [Required]
    [StringLength(50, MinimumLength = 3)]
    public string Username { get; set; } = default!;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string PasswordHash { get; set; } = default!;

    // Senha não deve ser armazenada diretamente
    [NotMapped]
    public string Password { get; set; } = default!;

    public User()
    {
        // Id já está inicializado acima
    }

    public void SetPassword(string password)
    {
        this.PasswordHash = BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password)
    {
        return BCrypt.Verify(password, this.PasswordHash);
    }
}

