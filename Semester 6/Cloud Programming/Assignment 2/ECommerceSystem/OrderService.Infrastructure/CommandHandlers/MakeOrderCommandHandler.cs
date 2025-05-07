using Common.Domain.Bus;
using MediatR;
using OrderService.Domain.Commands;
using OrderService.Domain.Entities;
using OrderService.Domain.Events;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.CommandHandlers
{
    public class MakeOrderCommandHandler : IRequestHandler<MakeOrderCommand, bool>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IEventBus _eventBus;
        
        public MakeOrderCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IEventBus eventBus)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _eventBus = eventBus;
        }

        public async Task<bool> Handle(MakeOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrder = new Order(request.UserId);
            await _orderRepository.CreateOrder(newOrder);

            foreach (var orderItem in request.OrderItems)
            {
                var newOrderItem = new OrderItem(newOrder.Id, orderItem.ProductId, orderItem.Quantity);
                await _orderItemRepository.CreateOrderItem(newOrderItem);
            }

            await _eventBus.Publish(new OrderCreatedEvent(newOrder.UserId, newOrder.Id));
            return true;
        }
    }
}