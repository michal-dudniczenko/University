using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Authentication;

internal sealed class AuthorizedUserAccessor(ApplicationDbContext db) : IAuthorizedUserAccessor
{
    public async Task<User?> GetAuthorizedUserAsync(
        ClaimsPrincipal principal,
        bool checkForFirstLogin,
        CancellationToken cancellationToken)
    {
        var subClaim = principal.FindFirst("sub")?.Value
            ?? principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (!Guid.TryParse(subClaim, out var userId))
            return null;

        var user = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

        if (user is null || user.IsLoggedOut || !user.IsEmailConfirmed)
            return null;

        if (checkForFirstLogin && user.IsFirstLogin)
            return null;

        return user;
    }
}
