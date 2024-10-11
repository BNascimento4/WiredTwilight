namespace WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Para NotMapped
using BCrypt.Net; // Para BCrypt

public class LoginRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}