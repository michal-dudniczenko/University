using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SoundmatesAPI.Database;
using SoundmatesAPI.DTOs;
using SoundmatesAPI.Models;
using SoundmatesAPI.Security;

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
    private readonly ILogger<UsersController> _logger;

    public UsersController(AppDbContext context, ISecretKeyProvider secretKeyProvider, ILogger<UsersController> logger)
    {
        _context = context;
        _secretKeyProvider = secretKeyProvider;
        _logger = logger;
    }

    // GET /users/profile
    [HttpGet("profile")]
    public async Task<ActionResult> GetUserProfile(
        [FromHeader(Name = "Access-Token")] string token)
    {
        _logger.LogWarning("GET /users/profile");
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var user = await _context.Users.FindAsync(authorizedUserId);
        if (user == null)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Description = user.Description,
            BirthYear = user.BirthYear,
            City = user.City,
            Country = user.Country,
            IsFirstLogin = user.IsFirstLogin
        });
    }

    // GET /users/{id}
    [HttpGet("{id}")]
    public async Task<ActionResult> GetUser(
        Guid id,
        [FromHeader(Name = "Access-Token")] string token)
    {
        _logger.LogWarning($"GET /users/{id}");
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
                IsFirstLogin = user.IsFirstLogin
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
        _logger.LogWarning("GET /users");
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
        _logger.LogWarning("POST /users/register");

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

        var accessToken = SecurityUtils.GenerateAccessToken(user.Id, SecretKey);

        return Ok(new AccessTokenDto { AccessToken = accessToken });
    }

    // PUT /users
    [HttpPut]
    public async Task<IActionResult> UpdateUser(
        [FromHeader(Name = "Access-Token")] string token,
        [FromBody] UpdateUserDto updateUserDto)
    {
        _logger.LogWarning("PUT /users");

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
            return BadRequest(new { message = "You can only update your own profile." });
        }

        user.Name = updateUserDto.Name;
        user.Description = updateUserDto.Description;
        user.BirthYear = updateUserDto.BirthYear;
        user.City = updateUserDto.City;
        user.Country = updateUserDto.Country;
        user.IsFirstLogin = false;

        _context.Users.Update(user);
        await _context.SaveChangesAsync();

        return Ok();
    }

    // POST /users/login
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginDto loginDto)
    {
        _logger.LogWarning("POST /users/login");

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Email == loginDto.Email);

        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

        if (result == PasswordVerificationResult.Failed)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        if (!user.IsActive && !user.IsFirstLogin)
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

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(30),
        };

        Response.Cookies.Append("refreshToken", refreshToken, cookieOptions);

        var accessToken = SecurityUtils.GenerateAccessToken(user.Id, SecretKey);

        return Ok(new AccessTokenDto { AccessToken = accessToken });
    }

    // POST /users/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(
        [FromHeader(Name = "Access-Token")] string token)
    {
        _logger.LogWarning("POST /users/logout");

        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var refreshToken = await _context.RefreshTokens.FindAsync(authorizedUserId);
        if (refreshToken == null)
        {
            return Ok(new { message = "User is already logged out." });
        }

        try
        {
            _context.RefreshTokens.Remove(refreshToken);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            _logger.LogWarning("RefreshToken already deleted concurrently.");
            // traktuj jako bezpieczne zakończenie
            return Ok(new { message = "Logged out (was already removed)." });
        }

        return Ok(new { message = "Logged out successfully." });
    }

    // POST /users/refresh
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh()
    {
        _logger.LogWarning("POST /users/refresh");

        var refreshTokenCookie = Request.Cookies["refreshToken"];

        if (refreshTokenCookie.IsNullOrEmpty())
        {
            return Unauthorized(new { message = "Missing refresh token in refreshToken cookie, log in to receive one." });
        }

        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenCookie);

        if (refreshToken == null || refreshToken.ExpirationDate < DateTime.UtcNow)
        {
            return Unauthorized(new { message = "Invalid or expired refresh token." });
        }

        var accessToken = SecurityUtils.GenerateAccessToken(refreshToken.UserId, SecretKey);

        return Ok(new AccessTokenDto { AccessToken = accessToken });
    }

    // DELETE /users
    [HttpDelete]
    public async Task<IActionResult> DeactivateUserAccount(
        [FromHeader(Name = "Access-Token")] string token,
        [FromBody] LoginDto loginDto)
    {
        _logger.LogWarning($"DELETE /users");

        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var user = await _context.Users.FindAsync(authorizedUserId);
        if (user == null)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);

        if (result == PasswordVerificationResult.Failed || user.Email != loginDto.Email)
        {
            return Unauthorized(new { message = "Invalid email or password." });
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
        _logger.LogWarning("POST /users/change-password");

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

        var newPasswordHash = _hasher.HashPassword(user, changePasswordDto.NewPassword);

        user.PasswordHash = newPasswordHash;
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
        return Ok(new { message = "Password changed successfully." });
    }
}
