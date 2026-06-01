using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Filters;
using Soundmates.Api.Common.Services;

namespace Soundmates.Api.Features.Auth.Login;

internal static class LoginEndpoint
{
    public static IEndpointRouteBuilder MapLogIn(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/login", HandleAsync)
            .WithName("LogIn")
            .WithSummary("Log in")
            .WithDescription("Validates credentials and returns access and refresh tokens.")
            .WithTags("Auth")
            .Produces<LoginResponse>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .Produces(StatusCodes.Status401Unauthorized)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status423Locked)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .RequireRateLimiting(SecurityConstants.RateLimitingAuthPolicyName)
            .AddEndpointFilter<ValidationFilter<LoginRequest>>()
            .AddEndpointFilter<ValidateCsrfTokenFilter>()
            .AllowAnonymous();

        return app;
    }

    private static async Task<IResult> HandleAsync(
        [FromQuery] bool useCookies,
        [FromBody] LoginRequest request,
        [FromServices] UserManager<User> userManager,
        [FromServices] SignInManager<User> signInManager,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);

        if (user is null)
            return TypedResults.Unauthorized();

        if (!user.IsActive)
        {
            return TypedResults.Problem(
                detail: "Your account has been deactivated. Contact administrator.",
                statusCode: StatusCodes.Status403Forbidden);
        }

        var result = useCookies ?
            await signInManager.PasswordSignInAsync(
                user, request.Password, isPersistent: true, lockoutOnFailure: true)
            : await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);

        if (result.IsLockedOut)
        {
            return TypedResults.Problem(
                title: "Account Locked",
                detail: "Account is temporarily locked due to too many failed sign-in attempts.",
                statusCode: StatusCodes.Status423Locked);
        }

        if (!result.Succeeded)
            return TypedResults.Unauthorized();

        if (useCookies)
            return TypedResults.Ok();

        var accessToken = await authService.GenerateAccessTokenAsync(user, cancellationToken);
        var refreshToken = await authService.CreateRefreshTokenAsync(user.Id, cancellationToken);

        return TypedResults.Ok(new LoginResponse(accessToken, refreshToken));
    }
}
