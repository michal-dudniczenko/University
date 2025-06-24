using Microsoft.EntityFrameworkCore;
using SoundmatesAPI.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddSingleton<ISecretKeyProvider>(provider =>
{
    using var scope = provider.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    return new SecretKeyProvider(context);
});

// Add services to the container.
builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    DbSeeder.SeedSecrets(context);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Soundmates API");
    });
}

app.UseAuthorization();

app.MapControllers();

app.Run();
