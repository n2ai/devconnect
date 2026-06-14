namespace DevConnect.Core.Models;

public class Follow
{
    public Guid Id { get; set; }
    public Guid FollowerId { get; set; }
    public Guid FollowingId { get; set; }
    public DateTime dateTime { get; set; } = DateTime.UtcNow;
    public User Follower { get; set; } = null!;
    public User Following { get; set; } = null!;
}