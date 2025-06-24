using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundmatesAPI.Database;
using SoundmatesAPI.DTOs;
using SoundmatesAPI.Models;
using SoundmatesAPI.Security;
using System.ComponentModel.DataAnnotations;

namespace SoundmatesAPI.Controllers;

[Route("users")]
[ApiController]
public class UsersController : ControllerBase
{
    const int MaxLimit = 100;
    private readonly PasswordHasher<User> _hasher = new PasswordHasher<User>();
    private string SecretKey => _secretKeyProvider.GetSecretKey();

    private readonly AppDbContext _context;
    private readonly ISecretKeyProvider _secretKeyProvider;

    public UsersController(AppDbContext context, ISecretKeyProvider secretKeyProvider)
    {
        _context = context;
        _secretKeyProvider = secretKeyProvider;
    }

    // GET /users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(
        Guid id,
        [FromHeader(Name = "Access-Token")] string token
        )
    {
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var user = await _context.Users.FindAsync(id);
        if (user == null || !user.IsActive)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        if (user.Id == authorizedUserId)
        {
            return Ok(new UserProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name,
                Description = user.Description,
                BirthYear = user.BirthYear,
                City = user.City,
                Country = user.Country,
            });
        } else
        {
            return Ok(new OtherUserProfileDto
            {
                Id = user.Id,
                Name = user.Name,
                Description = user.Description,
                BirthYear = user.BirthYear,
                City = user.City,
                Country = user.Country,
            });
        }
    }

    // GET /users?limit=20&offset=0
    [HttpGet]
    public async Task<IActionResult> GetUsers(
        [FromHeader(Name = "Access-Token")] string token,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        if (limit <= 0 || offset < 0)
        {
            return BadRequest(new { message = "Limit must be greater than 0 and offset cannot be negative." });
        }

        if (limit > MaxLimit)
        {
            return BadRequest(new { message = $"Limit cannot be greater than {MaxLimit}." });
        }

        var users = await _context.Users
            .Where(u => u.Id != authorizedUserId)
            .Where(u => u.IsActive == true)
            .Where(u =>
                !_context.Likes.Any(l => l.GiverId == authorizedUserId && l.ReceiverId == u.Id) &&
                !_context.Dislikes.Any(d => d.GiverId == authorizedUserId && d.ReceiverId == u.Id))
            .Skip(offset)
            .Take(limit)
            .Select(u => new OtherUserProfileDto
            {
                Id = u.Id,
                Name = u.Name,
                Description = u.Description,
                BirthYear = u.BirthYear,
                City = u.City,
                Country = u.Country
            })
            .ToListAsync();

        return Ok(users);
    }


    // POST /users/register
    [HttpPost("register")]
    public async Task<IActionResult> CreateUser(
        [FromBody] RegisterDto registerUserDto)
    {
        var existingUser = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == registerUserDto.Email);

        if (existingUser != null)
        {
            return BadRequest(new { message = "User with that email address already exists." });
        }

        var user = new User
        {
            Email = registerUserDto.Email,
        };

        var passwordHash = _hasher.HashPassword(user, registerUserDto.Password);

        user.PasswordHash = passwordHash;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // PUT /users
    [HttpPut]
    public async Task<IActionResult> UpdateUser(
        [FromHeader(Name = "Access-Token")] string token,
        [FromBody] UpdateUserDto updateUserDto)
    {
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var user = await _context.Users.FindAsync(updateUserDto.Id);
        if (user == null)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        if (user.Id != authorizedUserId)
        {
            return Unauthorized(new { message = "You can only update your own profile." });
        }

        user.Name = updateUserDto.Name;
        user.Description = updateUserDto.Description;
        user.BirthYear = updateUserDto.BirthYear;
        user.City = updateUserDto.City;
        user.Country = updateUserDto.Country;
        user.IsActive = true;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // POST /users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto loginDto)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
        {
            Console.WriteLine("test123");
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (!user.IsActive)
        {
            return Unauthorized(new { message = "Your account has been deactivated. Contact administrator" });
        }

        var refreshToken = SecurityUtils.GenerateRefreshToken();

        var existingRefreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.UserId == user.Id);

        if (existingRefreshToken != null)
        {
            existingRefreshToken.Token = refreshToken;
            existingRefreshToken.ExpirationDate = DateTime.UtcNow.AddDays(30);
            _context.RefreshTokens.Update(existingRefreshToken);
        }
        else
        {
            var newRefreshToken = new RefreshToken
            {
                UserId = user.Id,
                Token = refreshToken,
                ExpirationDate = DateTime.UtcNow.AddDays(7)
            };
            _context.RefreshTokens.Add(newRefreshToken);
        }
        await _context.SaveChangesAsync();

        var accessToken = SecurityUtils.GenerateAccessToken(user.Id, SecretKey);

        return Ok(new { refreshToken = refreshToken, accessToken = accessToken });
    }

    // POST /users/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromHeader(Name = "Access-Token")] string token)
    {
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var refreshToken = await _context.RefreshTokens.FindAsync(authorizedUserId);
        if (refreshToken == null)
        {
            return Ok(new { message = "User is already logged out." });
        }

        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Logged out successfully." });
    }

    // POST /users/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(
        [FromBody] RefreshTokenDto refreshTokenDto)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.Token);

        if (refreshToken == null || refreshToken.ExpirationDate < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token." });
        }

        var accessToken = SecurityUtils.GenerateAccessToken(refreshToken.UserId, SecretKey);

        return Ok(new { accessToken = accessToken });
    }

    // DELETE /users
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeactivateUserAccount(
        Guid id,
        [FromHeader(Name = "Access-Token")] string token
        )
    {
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var user = await _context.Users.FindAsync(id);
        if (user == null)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        if (user.Id != authorizedUserId)
        {
            return Unauthorized(new { message = "You can only deactivate your own account." });
        }

        user.IsActive = false;
        _context.Users.Update(user);

        var refreshToken = await _context.RefreshTokens.FindAsync(authorizedUserId);
        if (refreshToken != null)
        {
            _context.RefreshTokens.Remove(refreshToken);
        }

        await _context.SaveChangesAsync();
        return Ok(new { message = "Your account has been deactivated successfully." });
    }

    // POST /users/change-password
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword(
        [FromHeader(Name = "Access-Token")] string token,
        [FromBody] ChangePasswordDto changePasswordDto)
    {
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var user = await _context.Users.FindAsync(authorizedUserId);
        if (user == null)
        {
            return NotFound(new { message = "No user with specified id" });
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, changePasswordDto.OldPassword);

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Old password is incorrect." });
        }

        if (changePasswordDto.NewPassword != changePasswordDto.ConfirmNewPassword)
        {
            return BadRequest(new { message = "New password and confirmation do not match." });
        }

        var newPasswordHash = _hasher.HashPassword(user, changePasswordDto.NewPassword);

        user.PasswordHash = newPasswordHash;
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
        return Ok(new { message = "Password changed successfully." });
    }
}
