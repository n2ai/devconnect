namespace DevConnect.Core.Models;

public class Like
{
    public Guid UserID { get; set; }
    public Guid PostID { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public User User { get; set; } = null!;
    public Post Post { get; set; } = null!;
}