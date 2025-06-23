using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SoundmatesAPI.Database;
using SoundmatesAPI.DTOs;
using SoundmatesAPI.Models;
using SoundmatesAPI.Security;

namespace SoundmatesAPI.Controllers;

[Route("messages")]
[ApiController]
public class MessagesController : ControllerBase
{
    const int MaxLimit = 100;
    private readonly string _secretKey;

    private readonly AppDbContext _context;

    public MessagesController(AppDbContext context)
    {
        _context = context;
        var secret = _context.Secrets.OrderBy(s => s.SecretKey).FirstOrDefault();
        if (secret == null || string.IsNullOrEmpty(secret.SecretKey))
        {
            throw new InvalidOperationException("Secret key not found in the database.");
        }
        _secretKey = secret.SecretKey;
    }

    // GET /messages/preview?limit=20&offset=0
    [HttpGet("preview")]
    public async Task<IActionResult> GetMessagesPreview(
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var token = Request.Headers["Access-Token"].FirstOrDefault();
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { message = "Missing access token" });

        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, _secretKey);

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

        // Step 1: Get latest message per conversation
        var latestMessages = await _context.Messages
            .Where(m => m.SenderId == authorizedUserId || m.ReceiverId == authorizedUserId)
            .Select(m => new
            {
                Message = m,
                OtherUserId = m.SenderId == authorizedUserId ? m.ReceiverId : m.SenderId
            })
            .GroupBy(x => x.OtherUserId)
            .Select(g => g
                .OrderByDescending(x => x.Message.Timestamp)
                .First())
            .OrderByDescending(x => x.Message.Timestamp)
            .Skip(offset)
            .Take(limit)
            .ToListAsync();

        // Step 2: Get user names in memory
        var userIds = latestMessages.Select(x => x.OtherUserId).ToList();

        var userNames = await _context.Users
            .Where(u => userIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, u => u.Name);

        // Step 3: Map to DTOs
        var previews = latestMessages.Select(x => new MessagePreviewDto
        {
            Content = x.Message.Content,
            Timestamp = x.Message.Timestamp,
            SenderId = x.Message.SenderId,
            ReceiverId = x.Message.ReceiverId,
            UserName = userNames.TryGetValue(x.OtherUserId, out var name) ? name : "<Unknown>"
        }).ToList();

        return Ok(previews);
    }

    // GET /messages/{userId}?limit=20&offset=0
    [HttpGet("{userId}")]
    public async Task<IActionResult> GetMessages(
        Guid userId,
        [FromQuery] int limit = 20,
        [FromQuery] int offset = 0)
    {
        var token = Request.Headers["Access-Token"].FirstOrDefault();
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { message = "Missing access token" });

        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, _secretKey);

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

        var messages = await _context.Messages
            .Where(m => (m.SenderId == authorizedUserId && m.ReceiverId == userId) ||
                        (m.SenderId == userId && m.ReceiverId == authorizedUserId))
            .OrderByDescending(m => m.Timestamp)
            .Skip(offset)
            .Take(limit)
            .Select(m => new MessageDto
            {
                Id = m.Id,
                Content = m.Content,
                Timestamp = m.Timestamp,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId
            })
            .ToListAsync();

        return Ok(messages);
    }

    // POST /messages
    [HttpPost]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageDto sendMessageDto)
    {
        var token = Request.Headers["Access-Token"].FirstOrDefault();
        if (string.IsNullOrEmpty(token))
            return Unauthorized(new { message = "Missing access token" });

        var authorizedUserId = SecurityUtils.VerifyAccessToken(token, _secretKey);

        if (authorizedUserId == null)
            return Unauthorized(new { message = "Invalid access token" });

        var receiver = await _context.Users.FindAsync(sendMessageDto.ReceiverId);
        if (receiver == null)
        {
            return NotFound(new { message = "No user with specified id." });
        }

        var message = new Message
        {
            SenderId = authorizedUserId.Value,
            ReceiverId = sendMessageDto.ReceiverId,
            Content = sendMessageDto.Content
        };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync();

        return Ok();
    }
}
