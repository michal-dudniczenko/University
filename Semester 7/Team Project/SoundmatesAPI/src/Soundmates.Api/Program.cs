using FluentValidation;
using Scalar.AspNetCore;
using Soundmates.Api.Common.Constants;
using Soundmates.Api.Common.Hubs;
using Soundmates.Api.Extensions;
using Soundmates.Api.Middleware;
using Soundmates.Api.Persistence;
using System.Diagnostics;

Directory.CreateDirectory("wwwroot");

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddConfigureAuthentication(builder.Configuration);
builder.Services.AddConfigureAuthorization();
builder.Services.AddConfigureIdentity();
builder.Services.AddConfigureOptions(builder.Configuration);
builder.Services.AddConfigureCors(builder.Configuration);
builder.Services.AddConfigureRateLimiting();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddEmailService(builder.Configuration);
builder.Services.AddSignalR();

builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = SecurityConstants.CsrfTokenHeaderName;
    options.Cookie.Name = SecurityConstants.CsrfTokenCookieName;
    options.Cookie.HttpOnly = false;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.None;
});

builder.Services.AddExceptionHandler<BadHttpRequestExceptionHandler>();

builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = ctx =>
    {
        ctx.ProblemDetails.Extensions["traceId"] = Activity.Current?.Id ?? ctx.HttpContext.TraceIdentifier;
    };
});

if (builder.Environment.IsDevelopment())
    builder.Services.AddConfigureOpenApi();

// ====================================================================================

var app = builder.Build();

if (app.Environment.IsDevelopment())
    await app.InitializeMigrateDatabaseAsync();

await app.SeedApplicationAdminUserAsync();

app.EnsureStaticFilesDirectoriesExist();

app.UseExceptionHandler();

if (!app.Environment.IsDevelopment())
    app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.UseAntiforgery();

app.UseMiddleware<LogRequestInfoMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi().AllowAnonymous();
    app.MapScalarApiReference().AllowAnonymous();
}

app.MapHub<EventHub>("/eventHub");
app.MapFeatureEndpoints();
app.MapHealthChecks("/health").AllowAnonymous();

await app.RunAsync();
