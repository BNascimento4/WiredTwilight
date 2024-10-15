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
            // Configuração da entidade User
            modelBuilder.Entity<User>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<User>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<User>()
                .Property(u => u.PasswordHash)
                .IsRequired()
                .HasMaxLength(100);

            // Configuração da entidade Forum
            modelBuilder.Entity<Forum>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<Forum>()
                .Property(f => f.Title)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Forum>()
                .Property(f => f.Description)
                .HasMaxLength(500);

            // Configuração da entidade Post
            modelBuilder.Entity<Post>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Post>()
                .Property(p => p.Title)
                .IsRequired();

            modelBuilder.Entity<Post>()
                .Property(p => p.Content)
                .IsRequired();

            // Configuração da entidade Comment
            modelBuilder.Entity<Comment>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<Comment>()
                .Property(c => c.Content)
                .IsRequired();

            // Configuração da entidade Tag
            modelBuilder.Entity<Tag>()
                .HasKey(t => t.Id);

            modelBuilder.Entity<Tag>()
                .Property(t => t.Name)
                .IsRequired()
                .HasMaxLength(50);

            // Configuração da entidade PrivateMessage
            modelBuilder.Entity<PrivateMessage>()
                .HasKey(pm => pm.Id);

            modelBuilder.Entity<PrivateMessage>()
                .Property(pm => pm.Content)
                .IsRequired();

            // Configuração das relações
            modelBuilder.Entity<Post>()
                .HasOne(p => p.Forum)
                .WithMany(f => f.Posts)
                .HasForeignKey(p => p.ForumId);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId);

            modelBuilder.Entity<Post>()
                .HasMany(p => p.Tags)
                .WithMany(t => t.Posts);
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Forum> Forums { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<PrivateMessage> PrivateMessages { get; set; }
    }
}

