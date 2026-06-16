using DevConnect.Core.Models;
using DevConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevConnect.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class PostsController(AppDbContext db) : ControllerBase
{
    //Get USER UUID from JWT Token
    private Guid GetUserId() => Guid.Parse(User.Claims.First(c => c.Type ==  "userId").Value);

    //Create new post
    [HttpPost]
    public async Task<IActionResult> CreatePost(CreatePostRequest request)
    {
        var post = new Post
        {
            Id = Guid.NewGuid(),
            AuthorId = GetUserId(),
            Content = request.Content,
            Language = request.Language,
            CreatedAt = DateTime.UtcNow
        };

        db.Posts.Add(post);
        await db.SaveChangesAsync();

        return Ok(new { post.Id, post.Content, post.Language, post.CreatedAt });
    }

    // Get /api/post/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPost(Guid id)
    {
        var post = await db.Posts
            .Include(p => p.Author)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .ThenInclude(c => c.Author)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post == null) 
            return NotFound("Post not found.");
        
        return Ok(new
        {
            post.Id,
            post.Content,
            post.Language,
            post.CreatedAt,
            Author = new { post.Author.Id, post.Author.UserName },
            LikesCount = post.Likes.Count,
            Comments = post.Comments.Select(c => new
            {
                c.Id,
                c.Content,
                c.CreatedAt,
                Author = new {c.Author.Id, c.Author.UserName }
            })
        });

    }

    //Delete post
    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> DeletePost(Guid id)
    {
        var post = await db.Posts.FindAsync(id);
        
        if (post == null)
            return NotFound("Post not found.");
        
        if (post.AuthorId != GetUserId())
            return Forbid();
        
        db.Posts.Remove(post);
        await db.SaveChangesAsync();
        
        return Ok("Post deleted.");
    }
    
}

public record CreatePostRequest(string Content, string? Language);