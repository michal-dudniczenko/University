using Common.Domain.Bus;
using MediatR;
using ProductService.Domain.Commands;
using ProductService.Domain.Events;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.CommandHandlers;

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventBus _eventBus;

    public DeleteProductCommandHandler(IProductRepository productRepository, IEventBus eventBus)
    {
        _productRepository = productRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
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