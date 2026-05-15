using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.Validation;
using Soundmates.Api.Persistence;

namespace Soundmates.Api.Features.Auth.Register;

internal static class RegisterEndpoint
{
    public static IEndpointRouteBuilder MapRegister(this IEndpointRouteBuilder app)
    {
        app.MapPost("/auth/register", HandleAsync)
            .WithName("Register")
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account with the provided email and password.")
            .WithTags("Auth")
            .Produces(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .AllowAnonymous()
            .AddEndpointFilter<ValidationFilter<RegisterRequest>>();

        return app;
    }

    public static async Task<IResult> HandleAsync(
        [FromBody] RegisterRequest request,
        [FromServices] ApplicationDbContext db,
        [FromServices] IAuthService authService,
        CancellationToken cancellationToken)
    {
        var emailExists = await db.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        if (emailExists)
            return TypedResults.Problem(detail: "User with that email already exists.", statusCode: 400);

        var user = new User
        {
            Email = request.Email,
            PasswordHash = authService.GetPasswordHash(request.Password)
        };

        db.Users.Add(user);
        await db.SaveChangesAsync(cancellationToken);

        var defaultMatchPreference = new UserMatchPreference { UserId = user.Id };
        db.UserMatchPreferences.Add(defaultMatchPreference);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Created();
    }
}
