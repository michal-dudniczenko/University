using Common.Domain.Bus;
using MediatR;
using ProductService.Domain.Commands;
using ProductService.Domain.Entities;
using ProductService.Domain.Events;
using ProductService.Infrastracture.Repositories;

namespace ProductService.Infrastracture.CommandHandlers;

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, bool>
{
    private readonly IProductRepository _productRepository;
    private readonly IEventBus _eventBus;
    public CreateProductCommandHandler(IProductRepository productRepository, IEventBus eventBus)
    {
        _productRepository = productRepository;
        _eventBus = eventBus;
    }
    public async Task<bool> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
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