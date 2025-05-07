using Common.Domain.Bus;
using MediatR;
using ProductService.Domain.Commands;
using ProductService.Domain.Events;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.CommandHandlers;

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventBus _eventBus;

    public UpdateProductCommandHandler(IProductRepository productRepository, IEventBus eventBus)
    {
        _productRepository = productRepository;
        _eventBus = eventBus;
    }

    public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
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