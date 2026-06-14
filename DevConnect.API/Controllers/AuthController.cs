using Microsoft.AspNetCore.Mvc;
using DevConnect.Infrastructure.Data;
using DevConnect.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace DevConnect.API.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController(AppDbContext db, IConfiguration config) : ControllerBase
{
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request)
    {
        if (await db.Users.AnyAsync(u => u.Email == request.Email))
            return BadRequest("Email already in use.");
        
        if (await db.Users.AnyAsync(u => u.UserName == request.Username))
            return BadRequest("Username already taken.");
        
        //Create new user
        var user = new User
        {
            Id = Guid.NewGuid(),
            UserName = request.Username,
            Email = request.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password)

        };

        db.Users.Add(user);
        await db.SaveChangesAsync();

        return Ok(new { Token = GenerateToken(user) });

    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        //Find user by email
        var user = await db.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
        if(user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid email or password");
        }
        

        return Ok(new { Token = GenerateToken(user) });
    }

    private string GenerateToken(User user)
    {
        var key = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Secret"]!));
        var creds = new Microsoft.IdentityModel.Tokens.SigningCredentials(key, Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new System.Security.Claims.Claim("userId", user.Id.ToString()),
            new System.Security.Claims.Claim("username", user.UserName)
        };
        var token = new System.IdentityModel.Tokens.Jwt.JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler().WriteToken(token);
    }
}

// Request models
public record RegisterRequest(string Username, string Email, string Password);
public record LoginRequest(string Email, string Password);