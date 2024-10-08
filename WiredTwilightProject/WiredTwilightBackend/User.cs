namespace WiredTwilightBackend;

public class User
{
    public string Id;
    public string Username;
    public string Password;
    public User()
    {
        this.Id = Guid.NewGuid().ToString();
    }
}