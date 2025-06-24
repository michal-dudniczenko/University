using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundmatesAPI.Database;
using SoundmatesAPI.DTOs;
using SoundmatesAPI.Models;
using SoundmatesAPI.Security;

[Route("matching")]
[ApiController]
public class MatchingController : ControllerBase
{
    const int MaxLimit = 100;
    private string SecretKey => _secretKeyProvider.GetSecretKey();

    private readonly AppDbContext _context;
    private readonly ISecretKeyProvider _secretKeyProvider;

    public MatchingController(AppDbContext context, ISecretKeyProvider secretKeyProvider)
    {
        _context = context;
        _secretKeyProvider = secretKeyProvider;
    }

    // POST /matching/like
    [HttpPost("like")]
    public async Task<IActionResult> CreateLike(
        [FromHeader(Name = "Access-Token")] string token,
        [FromBody] SwipeDto swipeDto)
    {
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null || authorizedUserId != swipeDto.GiverId)
            return Unauthorized(new { message = "Invalid access token" });

        var giver = await _context.Users.FindAsync(swipeDto.GiverId);

        if (giver == null)
            return NotFound(new { message = $"No user with ID: {swipeDto.GiverId} was found." });

        var receiver = await _context.Users.FindAsync(swipeDto.ReceiverId);

        if (receiver == null)
            return NotFound(new { message = $"No user with ID: {swipeDto.ReceiverId} was found." });

        var likeExists = await _context.Likes
            .AnyAsync(l => l.GiverId == swipeDto.GiverId && l.ReceiverId == swipeDto.ReceiverId);

        if (!likeExists)
            _context.Likes.Add(new Like { GiverId = swipeDto.GiverId, ReceiverId = swipeDto.ReceiverId });

        var otherPersonLikeExists = await _context.Likes
            .AnyAsync(l => l.GiverId == swipeDto.ReceiverId && l.ReceiverId == swipeDto.GiverId);

        if (otherPersonLikeExists)
        {
            var matchExists = await _context.Matches
                .AnyAsync(m => (m.User1Id == swipeDto.GiverId && m.User2Id == swipeDto.ReceiverId) ||
                               (m.User1Id == swipeDto.ReceiverId && m.User2Id == swipeDto.GiverId));
            if (!matchExists)
            {
                _context.Matches.Add(new Match
                {
                    User1Id = swipeDto.GiverId,
                    User2Id = swipeDto.ReceiverId,
                });
            }
        }

        await _context.SaveChangesAsync();

        return Ok();
    }

    // POST /matching/dislike
    [HttpPost("dislike")]
    public async Task<IActionResult> CreateDislike(
        [FromHeader(Name = "Access-Token")] string token,
        [FromBody] SwipeDto swipeDto)
    {
        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, SecretKey);

        if (authorizedUserId == null || authorizedUserId != swipeDto.GiverId)
            return Unauthorized(new { message = "Invalid access token" });

        var giver = await _context.Users.FindAsync(swipeDto.GiverId);

        if (giver == null)
            return NotFound(new { message = $"No user with ID: {swipeDto.GiverId} was found." });

        var receiver = await _context.Users.FindAsync(swipeDto.ReceiverId);

        if (receiver == null)
            return NotFound(new { message = $"No user with ID: {swipeDto.ReceiverId} was found." });

        var dislikeExists = await _context.Dislikes
            .AnyAsync(dl => dl.GiverId == swipeDto.GiverId && dl.ReceiverId == swipeDto.ReceiverId);

        if (!dislikeExists)
            _context.Dislikes.Add(new Dislike { GiverId = swipeDto.GiverId, ReceiverId = swipeDto.ReceiverId });
            await _context.SaveChangesAsync();

        return Ok();
    }

    // GET /matching/matches?limit=20&offset=0
    [HttpGet("matches")]
    public async Task<IActionResult> GetMatches(
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

        var matches = await _context.Matches
            .Where(m => (m.User1Id == authorizedUserId || m.User2Id == authorizedUserId))
            .Where(m =>
                (m.User1Id == authorizedUserId && _context.Users.Any(u => u.Id == m.User2Id && u.IsActive)) ||
                (m.User2Id == authorizedUserId && _context.Users.Any(u => u.Id == m.User1Id && u.IsActive))
            )
            .OrderByDescending(m => m.Timestamp)
            .Skip(offset)
            .Take(limit)
            .Select(m => new MatchDto
            {
                UserId = m.User1Id == authorizedUserId ? m.User2Id : m.User1Id
            })
            .ToListAsync();

        return Ok(matches);
    }
}
