using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using BCrypt.Net;

namespace WiredTwilightBackend
{
    public class User
    {
        [Key]
        public string Id { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Username { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string PasswordHash { get; set; }

        // Senha não deve ser armazenada diretamente
        [NotMapped]
        public string Password { get; set; }

        public User()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public void SetPassword(string password)
        {
            this.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string password)
        {
            return BCrypt.Net.BCrypt.Verify(password, this.PasswordHash);
        }
    }
}
