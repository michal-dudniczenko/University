using FluentValidation;
using Scalar.AspNetCore;
using Soundmates.Api.Common;
using Soundmates.Api.Common.Exceptions;
using Soundmates.Api.Common.Hubs;
using Soundmates.Api.Common.Middleware;
using Soundmates.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddPersistence(builder.Configuration);

builder.Services.AddConfigureAuth(builder.Configuration);

builder.Services.AddConfigureCors(builder.Configuration);

builder.Services.AddValidatorsFromAssemblyContaining<Program>(includeInternalTypes: true);

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

builder.Services.AddSignalR();

builder.Services.AddEmailService();

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddConfigureOpenApi();
}

var app = builder.Build();

app.UseMiddleware<LogRequestInfoMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();

    await app.InitializeMigrateDatabaseAsync();
}

app.UseExceptionHandler();

app.UseCors(AppConstants.ClientAppCorsName);
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<EventHub>("/eventHub");

app.MapFeatureEndpoints();

await app.RunAsync();
