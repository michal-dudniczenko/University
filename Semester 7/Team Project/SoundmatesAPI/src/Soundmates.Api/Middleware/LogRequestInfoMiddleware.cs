namespace Soundmates.Api.Middleware;

internal sealed class LogRequestInfoMiddleware(
    RequestDelegate next,
    ILogger<LogRequestInfoMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        logger.LogInformation("REQUEST: {Method} {Url}",
            context.Request.Method,
            context.Request.Path + context.Request.QueryString);

        await next(context);

        logger.LogInformation("RESPONSE: {StatusCode} for {Method} {Url}",
                context.Response.StatusCode,
                context.Request.Method,
                context.Request.Path + context.Request.QueryString);
    }
}
