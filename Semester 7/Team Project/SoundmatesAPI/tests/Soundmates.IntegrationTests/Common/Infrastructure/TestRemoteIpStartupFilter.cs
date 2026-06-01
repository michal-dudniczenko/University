using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace Soundmates.IntegrationTests.Common.Infrastructure;

/// <summary>
/// Registered as an <see cref="IStartupFilter"/> so it wraps the whole pipeline (running before
/// UseRateLimiter). It overrides <see cref="ConnectionInfo.RemoteIpAddress"/> from the
/// <see cref="TestConstants.RemoteIpHeaderName"/> header, letting each test present a unique IP so
/// the per-IP auth rate limiter does not bleed across tests — while rate-limit tests pin one IP.
/// </summary>
internal sealed class TestRemoteIpStartupFilter : IStartupFilter
{
    public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
    {
        return app =>
        {
            app.Use(async (context, nextDelegate) =>
            {
                if (context.Request.Headers.TryGetValue(TestConstants.RemoteIpHeaderName, out var value)
                    && IPAddress.TryParse(value.ToString(), out var ip))
                {
                    context.Connection.RemoteIpAddress = ip;
                }

                await nextDelegate();
            });

            next(app);
        };
    }
}
