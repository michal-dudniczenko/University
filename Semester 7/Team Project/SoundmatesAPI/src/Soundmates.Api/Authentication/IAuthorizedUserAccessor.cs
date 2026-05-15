using Soundmates.Api.Common.Entities;
using System.Security.Claims;

namespace Soundmates.Api.Authentication;

internal interface IAuthorizedUserAccessor
{
    Task<User?> GetAuthorizedUserAsync(
        ClaimsPrincipal principal,
        bool checkForFirstLogin,
        CancellationToken cancellationToken);
}
