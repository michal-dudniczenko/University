using Common.Bus;
using Common.Domain.Bus;
using Common.Domain.Events.OrderService;
using Common.Domain.Events.UserService;
using MediatR;
using NotificationService.Api.Extensions;
using NotificationService.Domain.Commands;
using NotificationService.Infrastracture.EventHandlers;
using NotificationService.Infrastructure.CommandHandlers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// Add services to the container.

builder.Services.AddControllers();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new RabbitMQBus(scopeFactory);
});

builder.Services.AddTransient<OrderCreatedEventHandler>();
builder.Services.AddTransient<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();

builder.Services.AddTransient<UserRegisteredEventHandler>();
builder.Services.AddTransient<IEventHandler<UserRegisteredEvent>, UserRegisteredEventHandler>();

builder.Services.AddTransient<IRequestHandler<SendOrderNotificationCommand, bool>, SendOrderNotificationCommandHandler>();
builder.Services.AddTransient<IRequestHandler<SendRegisterNotificationCommand, bool>, SendRegisterNotificationCommandHandler>();

builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
        options.RoutePrefix = string.Empty;
    });
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.ConfigureEventBus();

app.Run();
