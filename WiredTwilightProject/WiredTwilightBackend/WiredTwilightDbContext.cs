
namespace WiredTwilightBackend;
using Microsoft.EntityFrameworkCore;
using Npgsql;
public class WiredTwilightDbContext : DbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql("Host=localhost;Database=WiredTwilightDB;Username=wired_t;Password=1111");
    }

    public DbSet<User> Users { get; set; }
}