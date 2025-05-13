using Common.Domain.Bus;
using Common.Domain.Events.ProductService;
using MediatR;
using Microsoft.Extensions.Logging;
using ProductService.Domain.Commands;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.CommandHandlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<DeleteProductCommandHandler> _logger;

    public DeleteProductCommandHandler(IProductRepository productRepository, IEventBus eventBus, ILogger<DeleteProductCommandHandler> logger)
    {
        _productRepository = productRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("DeleteProductCommand");

        var product = await _productRepository.GetProductById(request.Id);
        if (product == null)
        {
            return false;
        }
        await _productRepository.DeleteProduct(request.Id);
        await _eventBus.Publish(new ProductDeletedEvent(request.Id));
        return true;
    }
}