using Microsoft.Extensions.DependencyInjection;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;

namespace Soundmates.IntegrationTests.Common.Seeding;

/// <summary>Seeds relational rows (reactions, matches, messages, media, tokens) used by tests.</summary>
internal static class EntitySeeder
{
    public static Task<Guid> SeedLikeAsync(
        this CustomWebApplicationFactory factory, Guid giverId, Guid receiverId) =>
        factory.ExecuteDbContextAsync(async db =>
        {
            var like = new Like { GiverId = giverId, ReceiverId = receiverId };
            db.Likes.Add(like);
            await db.SaveChangesAsync();
            return like.Id;
        });

    public static Task<Guid> SeedDislikeAsync(
        this CustomWebApplicationFactory factory, Guid giverId, Guid receiverId) =>
        factory.ExecuteDbContextAsync(async db =>
        {
            var dislike = new Dislike { GiverId = giverId, ReceiverId = receiverId };
            db.Dislikes.Add(dislike);
            await db.SaveChangesAsync();
            return dislike.Id;
        });

    /// <summary>
    /// Seeds a match. Note ordering is not canonicalized in the app; pass the ids in whichever
    /// position the scenario requires (caller can be User1 or User2).
    /// </summary>
    public static Task<Guid> SeedMatchAsync(
        this CustomWebApplicationFactory factory, Guid user1Id, Guid user2Id) =>
        factory.ExecuteDbContextAsync(async db =>
        {
            var match = new Match { User1Id = user1Id, User2Id = user2Id };
            db.Matches.Add(match);
            await db.SaveChangesAsync();
            return match.Id;
        });

    public static Task<Guid> SeedMessageAsync(
        this CustomWebApplicationFactory factory,
        Guid senderId,
        Guid receiverId,
        string content = "Hello",
        bool isSeen = false,
        DateTime? createdAt = null) =>
        factory.ExecuteDbContextAsync(async db =>
        {
            var message = new Message
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = content,
                IsSeen = isSeen
            };
            if (createdAt.HasValue)
            {
                message.CreatedAt = createdAt.Value;
            }

            db.Messages.Add(message);
            await db.SaveChangesAsync();
            return message.Id;
        });

    public static Task<Guid> SeedMusicSampleAsync(
        this CustomWebApplicationFactory factory, Guid userId, string fileName, int displayOrder) =>
        factory.ExecuteDbContextAsync(async db =>
        {
            var sample = new MusicSample { UserId = userId, FileName = fileName, DisplayOrder = displayOrder };
            db.MusicSamples.Add(sample);
            await db.SaveChangesAsync();
            return sample.Id;
        });

    public static Task<Guid> SeedProfilePictureAsync(
        this CustomWebApplicationFactory factory, Guid userId, string fileName, int displayOrder) =>
        factory.ExecuteDbContextAsync(async db =>
        {
            var picture = new ProfilePicture { UserId = userId, FileName = fileName, DisplayOrder = displayOrder };
            db.ProfilePictures.Add(picture);
            await db.SaveChangesAsync();
            return picture.Id;
        });

    /// <summary>
    /// Seeds a refresh token row with an explicit expiry, hashing <paramref name="rawToken"/> the
    /// same way the app does. Returns the raw token so tests can POST it to /auth/refresh.
    /// </summary>
    public static Task<string> SeedRefreshTokenAsync(
        this CustomWebApplicationFactory factory,
        Guid userId,
        string? rawToken = null,
        DateTime? expiresAt = null) =>
        factory.ExecuteScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<ApplicationDbContext>();
            var authService = sp.GetRequiredService<IAuthService>();

            var token = rawToken ?? authService.GenerateRandomToken();
            db.RefreshTokens.Add(new RefreshToken
            {
                UserId = userId,
                TokenHash = authService.HashToken(token),
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddDays(7)
            });
            await db.SaveChangesAsync();
            return token;
        });

    /// <summary>Seeds a pending registration row (used to test confirm/resend/register purge logic).</summary>
    public static Task<string> SeedPendingRegistrationAsync(
        this CustomWebApplicationFactory factory,
        string email,
        string? rawToken = null,
        DateTime? expiresAt = null,
        string passwordHash = "dummy-hash") =>
        factory.ExecuteScopeAsync(async sp =>
        {
            var db = sp.GetRequiredService<ApplicationDbContext>();
            var authService = sp.GetRequiredService<IAuthService>();

            var token = rawToken ?? authService.GenerateRandomToken();
            db.PendingRegistrations.Add(new PendingRegistration
            {
                Email = email,
                PasswordHash = passwordHash,
                EmailTokenHash = authService.HashToken(token),
                ExpiresAt = expiresAt ?? DateTime.UtcNow.AddMinutes(30)
            });
            await db.SaveChangesAsync();
            return token;
        });
}
