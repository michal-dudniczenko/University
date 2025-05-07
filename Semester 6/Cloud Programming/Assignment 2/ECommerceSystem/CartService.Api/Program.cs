using CartService.Domain.Commands;
using CartService.Domain.DTO;
using CartService.Domain.Queries;
using CartService.Infrastructure.CommandHandlers;
using CartService.Infrastructure.QueryHandlers;
using CartService.Infrastructure.Repositories;
using Common.Bus;
using Common.Domain.Bus;
using MediatR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// Add services to the container.

builder.Services.AddControllers();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICartRepository, CartRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new RabbitMQBus(sp.GetService<IMediator>(), scopeFactory);
});

builder.Services.AddTransient<IRequestHandler<AddItemToCartCommand, bool>, AddItemToCartCommandHandler>();
builder.Services.AddTransient<IRequestHandler<RemoveItemFromCartCommand, bool>, RemoveItemFromCartCommandHandler>();
builder.Services.AddTransient<IRequestHandler<ClearUserCartCommand, bool>, ClearUserCartCommandHandler>();

builder.Services.AddTransient<IRequestHandler<GetUserCartItemsQuery, List<CartEntryDto>>, GetUserCartItemsQueryHandler>();

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
