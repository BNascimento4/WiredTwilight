using System.ComponentModel.DataAnnotations;

namespace WiredTwilightBackend;

public class User
{
    [Key]
    public string Id;
    public string Username;
    public string Password;
    public User()
    {
        this.Id = Guid.NewGuid().ToString();
        this.Username = string.Empty;
        this.Password = string.Empty;
    }
}