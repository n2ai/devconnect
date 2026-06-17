using DevConnect.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;   

namespace DevConnect.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FeedController(AppDbContext db) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetFeed(int page = 1, int limit = 20)
    {
        var posts = await db.Posts
            .Include(p => p.Author)
            .Include(p => p.Likes)
            .Include(p => p.Comments)
            .OrderByDescending(p => p.CreatedAt)
            .Skip((page - 1) * limit)
            .Take(limit)
            .Select(p => new
            {
                p.Id,
                p.Content,
                p.Language,
                p.CreatedAt,
                Author = new {p.Author.Id, p.Author.UserName },
                LikesCount = p.Likes.Count,
                CommentsCount = p.Comments.Count
            })
            .ToListAsync();
        
        var totalPosts = await db.Posts.CountAsync();

        return Ok(new
        {
            posts,
            page,
            limit,
            totalPosts,
            totalPages = (int)Math.Ceiling(totalPosts/ (double)limit)
        });
    }
}
