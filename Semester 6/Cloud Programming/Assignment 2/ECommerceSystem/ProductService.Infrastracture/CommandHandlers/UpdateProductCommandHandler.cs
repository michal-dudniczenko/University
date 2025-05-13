using Common.Domain.Bus;
using Common.Domain.Events.ProductService;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Commands;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.CommandHandlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<UpdateProductCommandHandler> _logger;

    public UpdateProductCommandHandler(IProductRepository productRepository, IEventBus eventBus, ILogger<UpdateProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("UpdateProductCommand");

        var product = await _productRepository.GetProductById(request.Id);
        if (product == null)
        {
            return false;
        }
        product.Name = request.Name;
        product.Description = request.Description;
        product.Price = request.Price;
        product.Quantity = request.Quantity;

        await _productRepository.UpdateProduct(product);
        await _eventBus.Publish(new ProductUpdatedEvent(product.Id));
        return true;
    }
}