using Microsoft.EntityFrameworkCore;
using DevConnect.Core.Models;  

namespace DevConnect.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<Like> Likes => Set<Like>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        // Follow - composite key 
        b.Entity<Follow>()
            .HasKey(f => new { f.FollowerId, f.FollowingId });
        
        b.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(u => u.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);
        
        b.Entity<Follow>()
            .HasOne(f => f.Following)
            .WithMany(u => u.Followers)
            .HasForeignKey(f => f.FollowingId)
            .OnDelete(DeleteBehavior.Restrict);

        // Like - composite key
        b.Entity<Like>()
            .HasKey(l => new { l.UserID, l.PostID });
        
        // Index for faster feed query
        b.Entity<Post>()
            .HasIndex(p => new { p.AuthorId, p.CreatedAt });
        
        // Username and Email must be unique
        b.Entity<User>()
            .HasIndex(u => u.UserName)
            .IsUnique();
        
        b.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();
    }
}