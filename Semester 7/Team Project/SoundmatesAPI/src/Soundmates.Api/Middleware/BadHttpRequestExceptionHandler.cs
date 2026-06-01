using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;

namespace Soundmates.Api.Middleware;

internal sealed class BadHttpRequestExceptionHandler(IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        if (exception is not BadHttpRequestException badHttpRequestException)
            return false;

        httpContext.Response.StatusCode = badHttpRequestException.StatusCode;

        await problemDetailsService.WriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = badHttpRequestException.StatusCode,
                Title = ReasonPhrases.GetReasonPhrase(badHttpRequestException.StatusCode) ?? "Bad Request",
                Detail = badHttpRequestException.Message
            }
        });

        return true;
    }
}
