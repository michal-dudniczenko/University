using Microsoft.EntityFrameworkCore;
using SoundmatesAPI.Models;

namespace SoundmatesAPI.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<MusicSample> MusicSamples { get; set; }
    public DbSet<ProfilePicture> ProfilePictures { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<Dislike> Dislikes { get; set; }
    public DbSet<Match> Matches { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<Secret> Secrets { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MusicSample>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(ms => ms.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ProfilePicture>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(pp => pp.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Message>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(l => l.GiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(l => l.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dislike>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(dl => dl.GiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Dislike>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(dl => dl.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.User1Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Match>()
            .HasOne<User>()
            .WithMany()
            .HasForeignKey(m => m.User2Id)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RefreshToken>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<RefreshToken>(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Like>()
            .HasIndex(l => new { l.GiverId, l.ReceiverId })
            .IsUnique();

        modelBuilder.Entity<Dislike>()
            .HasIndex(dl => new { dl.GiverId, dl.ReceiverId })
            .IsUnique();

        modelBuilder.Entity<Match>()
            .HasIndex(m => new { m.User1Id, m.User2Id })
            .IsUnique();

    }
}
