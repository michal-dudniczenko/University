using Common.Bus;
using Common.Domain;
using Common.Domain.Bus;
using Common.Domain.DTO;
using MediatR;
using Microsoft.EntityFrameworkCore;
using UserService.Domain.Commands;
using UserService.Domain.DTO;
using UserService.Domain.Queries;
using UserService.Infrastructure.CommandHandlers;
using UserService.Infrastructure.Helpers;
using UserService.Infrastructure.QueryHandlers;
using UserService.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5001);
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// Add services to the container.

builder.Services.AddControllers();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Constants.GetConnectionString("UserService")));

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new RabbitMQBus(scopeFactory);
});

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddTransient<IRequestHandler<AuthorizeUserCommand, bool>, AuthorizeUserCommandHandler>();
builder.Services.AddTransient<IRequestHandler<LoginUserCommand, bool>, LoginUserCommandHandler>();
builder.Services.AddTransient<IRequestHandler<RegisterUserCommand, bool>, RegisterUserCommandHandler>();

builder.Services.AddTransient<IRequestHandler<GetAllUsersQuery, List<UserDto>>, GetAllUsersQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetUserEmailQuery, UserEmailDto>, GetUserEmailQueryHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.

//app.UseHttpsRedirection();

app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

app.MapControllers();

app.Run();
