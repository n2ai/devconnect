namespace DevConnect.Core.Models;

public class Post
{
    public Guid Id { get; set; }
    public Guid AuthorId { get; set; }
    public string Content { get; set; } = string.Empty;
    public string? Language { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;  
    public User Author {get;set;} = null!;
    public ICollection<Like> Likes { get; set;} = [];
    public ICollection<Comment> Comments { get; set;} = [];
}