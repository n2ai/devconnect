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

public class FollowController(AppDbContext db) : ControllerBase
{
    private Guid GetUserId() => Guid.Parse(User.FindFirstValue("userId")!);
    //POST /api/follow/{userId}
    [HttpPost("{userId}")]
    public async Task<IActionResult> FollowUser(Guid userId)
    {
        var myId = GetUserId();

        //Check 1 wont allow user to follow itself
        if (myId == userId)
            return BadRequest("You cannot follow yourself");
        
        //Check 2 if user is exists
        var targetExists = await db.Users.AnyAsync(u => u.Id == userId);
        if (!targetExists)
            return NotFound("User not found");
        
        //Check 3 if already follow
        var alreadyFollowing = await db.Follows.AnyAsync(f => f.FollowerId == myId && f.FollowingId == userId);
        if(alreadyFollowing)
            return BadRequest("Already following this user");
        
        //Create follow record
        db.Follows.Add(new Follow
        {
            FollowerId = myId,
            FollowingId = userId
        });

        await db.SaveChangesAsync();

        return Ok("Followed Successfully");
    }

    // DELETE /api/follow/{userId} - unfollow 1 user
    [HttpDelete("{userId}")]
    public async Task<IActionResult> UnfollowUser(Guid userId)
    {
        var myId = GetUserId();

        var follow = await db.Follows.FirstOrDefaultAsync(f => f.FollowerId == myId && f.FollowingId == userId);
        if(follow == null)
            return NotFound("You are not following this user");

        db.Follows.Remove(follow);
        await db.SaveChangesAsync();
        
        return Ok("Unfollowed successfully");
    }

    // GET /api/follow/{userId}/followers
    [HttpGet("{userId}/followers")]
    public async Task<IActionResult> GetFollowers(Guid userId)
    {
        var followers = await db.Follows
            .Where(f => f.FollowingId == userId)
            .Include(f => f.Follower)
            .Select(f => new
            {
                f.Follower.Id,
                f.Follower.UserName,
                f.Follower.AvatarUrl
            })
            .ToListAsync();
        
        return Ok(followers);
    }
}


