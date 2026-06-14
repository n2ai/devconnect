namespace DevConnect.Core.Models;

public class User
{
    public Guid Id {get;set;}
    public string UserName { get; set;} = string.Empty;
    public string Email { get; set;} = string.Empty;
    public string PasswordHash { get; set;} = string.Empty;
    public string? Bio {get;set;}
    public string? AvatarUrl {get;set;}
    public DateTime CreatedAt {get;set;} = DateTime.UtcNow;

    public ICollection<Post> Posts { get; set;} = [];
    public ICollection<Follow> Following { get; set;} = [];
    public ICollection<Follow> Followers { get; set;} = [];
}