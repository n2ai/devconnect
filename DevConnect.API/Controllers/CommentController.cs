using DevConnect.Core.Models;
using DevConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DevConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CommentsController(AppDbContext db) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue("userId")!);

    //POST api/comments/{postId} 
    [HttpPost("{postId}")]
    public async Task<IActionResult> CreateComment(Guid postId, CreateCommentRequest request)
    {
        var userId = GetUserId();

        //Check if Post is exists
        var postExists = await db.Posts.AnyAsync(p=>p.Id == postId);
        if (!postExists)
        {
            return NotFound("Post not found");
        }

        var comment = new Comment
        {
            Id = Guid.NewGuid(),
            PostId = postId,
            AuthorId = userId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        db.Comments.Add(comment);
        await db.SaveChangesAsync();

        //Load author info Id
        var author = await db.Users.FindAsync(userId);

        return Ok(new
        {
            comment.Id,
            comment.Content,
            comment.CreatedAt,
            Author = new {author!.Id, author.UserName}
        });
    }

    //DELETE /api/comments/{commentId} - Delete comment
    [HttpDelete("{commentId}")]
    public async Task<IActionResult> DeleteComment(Guid commentId)
    {
        var userId = GetUserId();

        var comment = await db.Comments.FindAsync(commentId);

        if(comment == null) return NotFound("Comment not found");

        if(comment.AuthorId != userId) return Forbid();

        db.Comments.Remove(comment);
        await db.SaveChangesAsync();

        return Ok("Comment deleted");
    }

    //GET /api/comments/{postId} - Get all comments post 
    [HttpGet("{postId}")]
    public async Task<IActionResult>GetComments(Guid postId)
    {
        var postExists = await db.Posts.AnyAsync(p=>p.Id == postId);
        if(!postExists) return NotFound("Post not found");

        var comments = await db.Comments
            .Where(c=>c.PostId == postId)
            .Include(c=>c.Author)
            .OrderBy(c=>c.CreatedAt)
            .Select(c => new
            {
                c.Id,
                c.Content,
                c.CreatedAt,
                Author = new {c.Author.Id, c.Author.UserName, c.Author.AvatarUrl}
            })
            .ToListAsync();
        
        return Ok(comments);
    }
}

public record CreateCommentRequest (string Content);