using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;

namespace Soundmates.Api.Features.Auth.Logout;

internal static class LogoutEndpoint
{
    public static IEndpointRouteBuilder MapLogOut(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/logout", HandleAsync)
            .WithName("LogOut")
            .WithSummary("Log out the authenticated user")
            .WithDescription("Signs out the current cookie session.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status204NoContent)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidateCsrfTokenFilter>();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromServices] SignInManager<User> signInManager)
    {
        await signInManager.SignOutAsync();
        return TypedResults.NoContent();
    }
}
