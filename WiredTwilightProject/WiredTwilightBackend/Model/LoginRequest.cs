namespace WiredTwilightBackend;

using WiredTwilightBackend;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;

public class LoginRequest
{
    public string? Username { get; set; } = null;
    public string? Password { get; set; } = null;
}