using Common.Bus;
using Common.Domain.Bus;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Commands;
using OrderService.Domain.DTO;
using OrderService.Domain.Queries;
using OrderService.Infrastracture.Repositories;
using OrderService.Infrastructure.CommandHandlers;
using OrderService.Infrastructure.QueryHandlers;
using OrderService.Infrastructure.Repositories;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// Add services to the container.

builder.Services.AddControllers();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderItemRepository, OrderItemRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new RabbitMQBus(sp.GetService<IMediator>(), scopeFactory);
});

builder.Services.AddTransient<IRequestHandler<MakeOrderCommand, bool>, MakeOrderCommandHandler>();

builder.Services.AddTransient<IRequestHandler<GetOrderItemsQuery, List<OrderItemDto>>, GetOrderItemsQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetUserOrdersQuery, List<OrderDto>>, GetUserOrdersQueryHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

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
