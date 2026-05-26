using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Metadata;
using Soundmates.Api.Common.Constants;
using System.Reflection;

namespace Soundmates.Api.Common.Filters;

[AttributeUsage(AttributeTargets.Method)]
internal sealed class ValidateCsrfTokenAttribute : Attribute { }

internal sealed class ValidateCsrfTokenFilter(IAntiforgery antiforgery) : IEndpointFilter, IEndpointMetadataProvider
{
    public static void PopulateMetadata(MethodInfo method, EndpointBuilder builder)
    {
        builder.Metadata.Add(new ValidateCsrfTokenAttribute());
    }

    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        // Skip CSRF for JWT-authenticated requests
        if (context.HttpContext.User.Identity?.AuthenticationType == JwtBearerDefaults.AuthenticationScheme)
            return await next(context);

        // Only enforce CSRF when an auth cookie is present — i.e., cookie-authenticated requests
        if (!context.HttpContext.Request.Cookies.ContainsKey(SecurityConstants.AuthCookieName))
            return await next(context);

        try
        {
            await antiforgery.ValidateRequestAsync(context.HttpContext);
        }
        catch (AntiforgeryValidationException)
        {
            return TypedResults.Problem(
                title: "CSRF validation failed",
                detail: "Cookie-authenticated requests must include a valid CSRF token in the X-CSRF-TOKEN header.",
                statusCode: StatusCodes.Status400BadRequest
            );
        }

        return await next(context);
    }
}
