using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;
using System.Security.Claims;

namespace Soundmates.Api.Features.Messages.GetConversation;

internal static class GetConversationEndpoint
{
    private const int MaxLimit = 50;

    public static IEndpointRouteBuilder MapGetConversation(this IEndpointRouteBuilder app)
    {
        app.MapGet("/messages/{otherUserId}", HandleAsync)
            .WithName("GetConversation")
            .WithSummary("Get conversation messages")
            .WithDescription("Returns paginated messages between the current user and another matched user.")
            .WithTags("Messages")
            .Produces<List<MessageResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .Produces(StatusCodes.Status404NotFound)
            .RequireAuthorization();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromRoute] string otherUserId,
        [FromQuery] int limit,
        [FromQuery] int offset,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var guidErrors = GuidValidator.ValidateGuid(otherUserId, fieldName: "otherUserId");
        if (guidErrors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(guidErrors));

        var otherUserGuid = Guid.Parse(otherUserId);

        var paginationErrors = PaginationValidator.ValidateLimitOffset(limit, offset, MaxLimit);
        if (paginationErrors is not null)
            return TypedResults.UnprocessableEntity(new ValidationProblemDetails(paginationErrors));

        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: true, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var otherUser = await db.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == otherUserGuid, cancellationToken);

        if (otherUser is null || otherUser.IsFirstLogin)
            return TypedResults.Problem(detail: $"User with id {otherUserId} not found.", statusCode: 404);

        var messages = await db.Messages
            .AsNoTracking()
            .Where(m => (m.SenderId == user.Id && m.ReceiverId == otherUserGuid)
                     || (m.SenderId == otherUserGuid && m.ReceiverId == user.Id))
            .OrderBy(m => m.Timestamp)
            .Skip(offset)
            .Take(limit)
            .Select(m => new MessageResponse(m.Content, m.Timestamp, m.SenderId, m.ReceiverId, m.IsSeen))
            .ToListAsync(cancellationToken);

        return TypedResults.Ok(messages);
    }
}
