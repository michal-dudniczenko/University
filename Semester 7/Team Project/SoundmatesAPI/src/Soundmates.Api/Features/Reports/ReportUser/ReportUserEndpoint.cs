using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Common.Validation;
using System.Security.Claims;

namespace Soundmates.Api.Features.Reports.ReportUser;

internal static class ReportUserEndpoint
{
    public static IEndpointRouteBuilder MapReportUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/reports", HandleAsync)
            .WithName("ReportUser")
            .WithSummary("Report a user")
            .WithDescription("Submits a user report that is sent via email to the moderation team.")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .WithTags("Reports")
            .RequireAuthorization()
            .AddEndpointFilter<ValidationFilter<ReportUserRequest>>();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] ReportUserRequest request,
        [FromServices] IAuthorizedUserAccessor authorizedUser,
        [FromServices] IEmailService emailService,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var user = await authorizedUser.GetAuthorizedUserAsync(principal, checkForFirstLogin: false, cancellationToken);
        if (user is null)
            return TypedResults.Unauthorized();

        var reportedUserId = Guid.Parse(request.ReportedUserId);

        var subject = $"User Report: {user.Id} reported {reportedUserId}";
        var body = $"""
            <h1>User Report</h1>
            <p><strong>Reporting User ID:</strong> {user.Id}</p>
            <p><strong>Reported User ID:</strong> {reportedUserId}</p>
            <p><strong>Reason:</strong> {request.Reason}</p>
            <p><strong>Description:</strong> {request.Description}</p>
            """;

        await emailService.SendEmailAsync("soundmatesmoderation@gmail.com", subject, body);

        return TypedResults.Ok();
    }
}
