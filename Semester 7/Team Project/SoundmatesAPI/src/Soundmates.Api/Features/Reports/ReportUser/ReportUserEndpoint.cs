using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Soundmates.Api.Features.Reports.ReportUser;

internal static class ReportUserEndpoint
{
    public static IEndpointRouteBuilder MapReportUser(this IEndpointRouteBuilder app)
    {
        app.MapPost("/reports", HandleAsync)
            .WithName("ReportUser")
            .WithSummary("Report a user")
            .WithDescription("Submits a user report that is sent via email to the moderation team.")
            .WithTags("Reports")
            .Produces(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .AddEndpointFilter<ValidationFilter<ReportUserRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromBody] ReportUserRequest request,
        [FromServices] IAuthService authService,
        [FromServices] IEmailService emailService,
        IConfiguration configuration,
        ClaimsPrincipal principal,
        CancellationToken cancellationToken)
    {
        var moderationEmail = configuration[ApplicationConstants.ModerationEmailConfigEntryName]
            ?? throw new InvalidOperationException($"{ApplicationConstants.ModerationEmailConfigEntryName} is not configured.");
        
        var user = await authService.GetAuthorizedUserAsync(principal);
        if (user is null)
            return TypedResults.Unauthorized();

        var reportedUserId = Guid.Parse(request.ReportedUserId);

        var encoder = HtmlEncoder.Default;

        var subject = $"User Report: {user.Id} reported {reportedUserId}";
        var body = $"""
            <h1>User Report</h1>
            <p><strong>Reporting User ID:</strong> {encoder.Encode(user.Id.ToString())}</p>
            <p><strong>Reported User ID:</strong> {encoder.Encode(reportedUserId.ToString())}</p>
            <p><strong>Reason:</strong> {encoder.Encode(request.Reason)}</p>
            <p><strong>Description:</strong> {encoder.Encode(request.Description)}</p>
            """;

        await emailService.SendEmailAsync(moderationEmail, subject, body, cancellationToken);

        return TypedResults.Ok();
    }
}
