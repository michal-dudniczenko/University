using Common.Domain.Bus;
using Common.Domain.Events.ProductService;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Commands;
using ProductService.Domain.Entities;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.CommandHandlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<CreateProductCommandHandler> _logger;
    public CreateProductCommandHandler(IProductRepository productRepository, IEventBus eventBus, ILogger<CreateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _eventBus = eventBus;
        _logger = logger;
    }
    public async Task<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("CreateProductCommand");

        var newProduct = new Product
        {
            Name = request.Name,
            Description = request.Description,
            Price = request.Price,
            Quantity = request.Quantity
        };

        await _productRepository.AddProduct(newProduct);
        await _eventBus.Publish(new ProductCreatedEvent(newProduct.Id));
        return true;
    }

}