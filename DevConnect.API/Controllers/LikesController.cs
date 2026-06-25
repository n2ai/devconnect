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

public class LikesController(AppDbContext db) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue("userId")!);

    //POST /api/likes/{postId} - like 1 post
    [HttpPost("{postId}")]
    public async Task<IActionResult> LikePost(Guid postId)
    {
        var userId = GetUserId();

        //Check if post is existed
        var postExists = await db.Posts.AnyAsync(p => p.Id == postId);
        if (!postExists)
        {
            return NotFound("Post not found");
        }

        //Check if like or not
        var alreadyLiked = await db.Likes.AnyAsync(l => l.UserID == userId && l.PostID == postId);
        
        if(alreadyLiked) return BadRequest("Already liked this post");

        db.Likes.Add(new Like {
            UserID = userId,
            PostID = postId
        });

        await db.SaveChangesAsync();

        return Ok("Liked successfully");
    }

    //DELETE /api/likes/{postId} - unlike 1 post
    [HttpDelete("{postId}")]
    public async Task<IActionResult> UnlikePost(Guid postId)
    {
        var userId = GetUserId();

        var like = await db.Likes.FirstOrDefaultAsync(l => l.UserID == userId && l.PostID == postId);

        if (like == null) return NotFound("You have not liked this post");

        db.Likes.Remove(like);
        await db.SaveChangesAsync();

        return Ok("Unliked successfully");
    }
}