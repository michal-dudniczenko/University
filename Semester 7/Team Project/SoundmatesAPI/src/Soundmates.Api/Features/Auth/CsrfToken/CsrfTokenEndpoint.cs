using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Soundmates.Api.Common.Constants;

namespace Soundmates.Api.Features.Auth.CsrfToken;

internal static class CsrfTokenEndpoint
{
    public static IEndpointRouteBuilder MapCsrfToken(this IEndpointRouteBuilder app)
    {
        app.MapGet("/auth/csrf-token", Handle)
            .WithName("CsrfToken")
            .WithSummary("Get a CSRF token")
            .WithDescription("Issues a CSRF token required for state-mutating requests.")
            .WithTags("Auth")
            .Produces<CsrfTokenResponse>(StatusCodes.Status200OK)
            .AllowAnonymous();

        return app;
    }

    private static Ok<CsrfTokenResponse> Handle(
        [FromServices] IAntiforgery antiforgery,
        HttpContext httpContext)
    {
        var tokens = antiforgery.GetAndStoreTokens(httpContext);

        if (tokens.RequestToken is null)
            throw new InvalidOperationException("Antiforgery request token was null.");

        httpContext.Response.Headers.CacheControl = "no-store";
        httpContext.Response.Headers.Pragma = "no-cache";

        return TypedResults.Ok(new CsrfTokenResponse(
            Token: tokens.RequestToken,
            HeaderName: SecurityConstants.CsrfTokenHeaderName,
            CookieName: SecurityConstants.CsrfTokenCookieName
        ));
    }
}
