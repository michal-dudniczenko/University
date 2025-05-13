using CartService.Api.Extensions;
using CartService.Domain.Commands;
using CartService.Domain.Queries;
using CartService.Infrastracture.EventHandlers;
using CartService.Infrastructure.CommandHandlers;
using CartService.Infrastructure.QueryHandlers;
using CartService.Infrastructure.Repositories;
using Common.Bus;
using Common.Domain;
using Common.Domain.Bus;
using Common.Domain.DTO;
using Common.Domain.Events.OrderService;
using Common.Domain.Events.ProductService;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Konfiguracja Kestrel
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5003);
});

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// Add services to the container.

builder.Services.AddControllers();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Constants.GetConnectionString("CartService")));

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new RabbitMQBus(scopeFactory);
});

builder.Services.AddTransient<OrderCreatedEventHandler>();
builder.Services.AddTransient<ProductDeletedEventHandler>();
builder.Services.AddTransient<ProductUpdatedEventHandler>();
builder.Services.AddTransient<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();
builder.Services.AddTransient<IEventHandler<ProductDeletedEvent>, ProductDeletedEventHandler>();
builder.Services.AddTransient<IEventHandler<ProductUpdatedEvent>, ProductUpdatedEventHandler>();

builder.Services.AddTransient<IRequestHandler<AddItemToCartCommand, bool>, AddItemToCartCommandHandler>();
builder.Services.AddTransient<IRequestHandler<RemoveItemFromCartCommand, bool>, RemoveItemFromCartCommandHandler>();
builder.Services.AddTransient<IRequestHandler<ClearUserCartCommand, bool>, ClearUserCartCommandHandler>();
builder.Services.AddTransient<IRequestHandler<ClearCartsContainingProductCommand, bool>, ClearCartsContainingProductCommandHandler>();

builder.Services.AddTransient<IRequestHandler<GetUserCartItemsQuery, List<CartEntryDto>>, GetUserCartItemsQueryHandler>();

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

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.ConfigureEventBus();

app.Run();
