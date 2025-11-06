using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ChatGroup> ChatGroups { get; set; }
        public DbSet<ChatGroupMember> ChatGroupMembers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Composite key for ChatGroupMember
            modelBuilder.Entity<ChatGroupMember>()
                .HasKey(gm => new { gm.UserId, gm.ChatGroupId });

            // Relationships
            modelBuilder.Entity<ChatGroupMember>()
                .HasOne(gm => gm.User)
                .WithMany(u => u.GroupMemberships)
                .HasForeignKey(gm => gm.UserId);

            modelBuilder.Entity<ChatGroupMember>()
                .HasOne(gm => gm.ChatGroup)
                .WithMany(g => g.Members)
                .HasForeignKey(gm => gm.ChatGroupId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.Messages)
                .HasForeignKey(m => m.SenderId);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Group)
                .WithMany(g => g.Messages)
                .HasForeignKey(m => m.GroupId);

            // ======== SEED DATA ========

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Username = "Alice", PasswordHash = "MOfFeyR+dJdD998daoFvZjlS3Zc9N6POz65+RRARhoOgX4pCTNvWCTIANdj9ti6p" },
                new User { Id = 2, Username = "Bob", PasswordHash = "MOfFeyR+dJdD998daoFvZjlS3Zc9N6POz65+RRARhoOgX4pCTNvWCTIANdj9ti6p" },
                new User { Id = 3, Username = "Charlie", PasswordHash = "MOfFeyR+dJdD998daoFvZjlS3Zc9N6POz65+RRARhoOgX4pCTNvWCTIANdj9ti6p" },
                new User { Id = 4, Username = "Joseph", PasswordHash = "MOfFeyR+dJdD998daoFvZjlS3Zc9N6POz65+RRARhoOgX4pCTNvWCTIANdj9ti6p" },
                new User { Id = 5, Username = "Diana", PasswordHash = "MOfFeyR+dJdD998daoFvZjlS3Zc9N6POz65+RRARhoOgX4pCTNvWCTIANdj9ti6p" }
            );

            modelBuilder.Entity<ChatGroup>().HasData(
                new ChatGroup { Id = 1, Name="random", CreatedAt = DateTime.UtcNow },
                new ChatGroup { Id = 2, Name="software", CreatedAt = DateTime.UtcNow },
                new ChatGroup { Id = 3, Name="Testing",CreatedAt = DateTime.UtcNow.AddDays(-1) },
                new ChatGroup { Id = 4, Name="Movies",CreatedAt = DateTime.UtcNow.AddDays(-1) }
            );

            modelBuilder.Entity<ChatGroupMember>().HasData(
                new ChatGroupMember { UserId = 1, ChatGroupId = 1, JoinedAt = DateTime.UtcNow },
                new ChatGroupMember { UserId = 2, ChatGroupId = 1, JoinedAt = DateTime.UtcNow },
                new ChatGroupMember { UserId = 3, ChatGroupId = 1, JoinedAt = DateTime.UtcNow },
                new ChatGroupMember { UserId = 4, ChatGroupId = 1, JoinedAt = DateTime.UtcNow },

                new ChatGroupMember { UserId = 2, ChatGroupId = 2, JoinedAt = DateTime.UtcNow },
                new ChatGroupMember { UserId = 1, ChatGroupId = 2, JoinedAt = DateTime.UtcNow },
                new ChatGroupMember { UserId = 4, ChatGroupId = 2, JoinedAt = DateTime.UtcNow },
                new ChatGroupMember { UserId = 5, ChatGroupId = 2, JoinedAt = DateTime.UtcNow },

                new ChatGroupMember { UserId = 4, ChatGroupId = 3, JoinedAt = DateTime.UtcNow.AddDays(-12) },
                new ChatGroupMember { UserId = 4, ChatGroupId = 4, JoinedAt = DateTime.UtcNow.AddDays(-1) }
            );

            modelBuilder.Entity<Message>().HasData(
                new Message { Id = 1, GroupId = 1, SenderId = 1, Content = "Hi Bob!", CreatedAt = DateTime.UtcNow },
                new Message { Id = 2, GroupId = 1, SenderId = 2, Content = "Hey Alice!", CreatedAt = DateTime.UtcNow },
                new Message { Id = 3, GroupId = 1, SenderId = 2, Content = "Hello Charlie!", CreatedAt = DateTime.UtcNow },
                new Message { Id = 4, GroupId = 2, SenderId = 1, Content = "Hi Bob!", CreatedAt = DateTime.UtcNow },
                new Message { Id = 5, GroupId = 2, SenderId = 2, Content = "Hey Alice!", CreatedAt = DateTime.UtcNow },
                new Message { Id = 6, GroupId = 2, SenderId = 2, Content = "Hello Charlie!", CreatedAt = DateTime.UtcNow }
            );

            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, UserId = 1, Name = "Apple", Price = 5 },
                new Product { Id = 2, UserId = 2, Name = "Apple", Price = 5 }
            );
        }
    }
}
