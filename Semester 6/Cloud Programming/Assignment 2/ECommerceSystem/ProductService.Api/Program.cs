using Common.Bus;
using Common.Domain;
using Common.Domain.Bus;
using Common.Domain.Events.OrderService;
using MediatR;
using Microsoft.EntityFrameworkCore;
using ProductService.Api.Extensions;
using ProductService.Domain.Commands;
using ProductService.Domain.DTO;
using ProductService.Domain.Queries;
using ProductService.Infrastracture.CommandHandlers;
using ProductService.Infrastracture.EventHandlers;
using ProductService.Infrastracture.QueryHandlers;
using ProductService.Infrastracture.Repositories;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(typeof(Program).Assembly));

// Add services to the container.

builder.Services.AddControllers();

// swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(Constants.GetConnectionString("ProductService")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();

builder.Services.AddSingleton<IEventBus, RabbitMQBus>(sp =>
{
    var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
    return new RabbitMQBus(scopeFactory);
});

builder.Services.AddTransient<OrderCreatedEventHandler>();
builder.Services.AddTransient<IEventHandler<OrderCreatedEvent>, OrderCreatedEventHandler>();

builder.Services.AddTransient<IRequestHandler<CreateProductCommand, bool>, CreateProductCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateProductCommand, bool>, UpdateProductCommandHandler>();
builder.Services.AddTransient<IRequestHandler<DeleteProductCommand, bool>, DeleteProductCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateProductsQuantityCommand, bool>, UpdateProductsQuantityCommandHandler>();

builder.Services.AddTransient<IRequestHandler<GetAllProductsQuery, List<ProductDto>>, GetAllProductsQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetProductByIdQuery, ProductDto>, GetProductByIdQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetAvailableProductsQuery, List<ProductDto>>, GetAvailableProductsQueryHandler>();

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
