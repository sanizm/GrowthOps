using GrowthOps.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using GrowthOps.Api.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace GrowthOps.Api.Services;

public class AuthService
{
  private readonly GrowthOpsDbContext _db;
  private readonly PasswordHasher<User> _hasher;

  public AuthService(GrowthOpsDbContext db)
  {
    _db = db;
    _hasher = new PasswordHasher<User>();
  }

  public async Task<User> RegisterAsync(string email, string password)
  {
    var exists = await _db.Users.AnyAsync(u => u.Email == email);
    if (exists)
      throw new ArgumentException("Email already exists.");

    var user = new User
    {
      Email = email
    };

    user.PasswordHash = _hasher.HashPassword(user, password);

    _db.Users.Add(user);
    await _db.SaveChangesAsync();

    return user;
  }

  public async Task<User?> LoginAsync(string email, string password)
  {
    var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == email);
    if (user is null)
      return null;

    var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, password);

    if (result == PasswordVerificationResult.Failed)
      return null;

    return user;
  }

  public string GenerateToken(User user, IConfiguration config)
  {
    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Email, user.Email)
    };

    var key = new SymmetricSecurityKey(
        Encoding.UTF8.GetBytes(config["Jwt:Key"]!)
    );

    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        issuer: config["Jwt:Issuer"],
        audience: config["Jwt:Audience"],
        claims: claims,
        expires: DateTime.UtcNow.AddHours(2),
        signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}