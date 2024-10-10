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
                .IsRequired() // Define como obrigatória
                .HasMaxLength(50); // Define um comprimento máximo

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired() // Define como obrigatória
                .HasMaxLength(100); // Define um comprimento máximo

            // Remova ou comente a configuração abaixo, pois 'Password' não está mapeada
            /*
            modelBuilder.Entity<User>()
                .Property(u => u.Password)
                .IsRequired() // Define como obrigatória
                .HasMaxLength(100); // Define um comprimento máximo
            */
        }

        public DbSet<User> Users { get; set; }
    }
}

