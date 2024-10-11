using Microsoft.EntityFrameworkCore;

namespace WiredTwilightBackend
{
    public class WiredTwilightDbContext : DbContext
    {
        // Construtor que aceita DbContextOptions
        public WiredTwilightDbContext(DbContextOptions<WiredTwilightDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            // Configurações adicionais para a entidade User
            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(100);

            // Removido: Configuração para 'Password' que é [NotMapped]
            // modelBuilder.Entity<User>()
            //     .Property(u => u.Password)
            //     .IsRequired()
            //     .HasMaxLength(100);
        }


        public DbSet<User> Users { get; set; }
    }
}

