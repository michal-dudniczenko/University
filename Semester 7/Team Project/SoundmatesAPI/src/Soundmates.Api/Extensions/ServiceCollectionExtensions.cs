using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Soundmates.Api.Authentication;
using Soundmates.Api.Common;
using Soundmates.Api.Common.Entities;
using Soundmates.Api.Common.OpenApiTransformers;
using Soundmates.Api.Common.Services;
using Soundmates.Api.Persistence;
using Soundmates.Api.Persistence.DataSeeding;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

namespace Soundmates.Api.Extensions;

internal static class ServiceCollectionExtensions
{
    public static IServiceCollection AddConfigureAuth(this IServiceCollection services, IConfiguration configuration)
    {
        var secretKey = configuration["Jwt:SecretKey"];
        if (string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("SecretKey is not configured");

        JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];
                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/eventHub", StringComparison.OrdinalIgnoreCase))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();

        services.AddScoped<IAuthService>(_ => new AuthService(secretKey));
        services.AddScoped<IAuthorizedUserAccessor, AuthorizedUserAccessor>();

        return services;
    }

    public static IServiceCollection AddConfigureCors(this IServiceCollection services, IConfiguration configuration)
    {
        var allowedOrigin = configuration["CorsAllowedUrl"];
        if (string.IsNullOrEmpty(allowedOrigin))
            throw new InvalidOperationException("CorsAllowedUrl is not configured");

        services.AddCors(options =>
        {
            options.AddPolicy(AppConstants.ClientAppCorsName, policy =>
            {
                policy.WithOrigins([allowedOrigin])
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        return services;
    }

    public static IServiceCollection AddConfigureOpenApi(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
        });

        return services;
    }

    public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException("Database connection string is not configured.");
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options
                .UseSqlServer(connectionString)
                .UseAsyncSeeding(async (context, _, ct) =>
                {
                    if (!await context.Set<Country>().AnyAsync(cancellationToken: ct))
                    {
                        await SeedingScripts.SeedData(context, ct);
                    }
                });
        });

        return services;
    }

    public static IServiceCollection AddEmailService(this IServiceCollection services)
    {
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}
