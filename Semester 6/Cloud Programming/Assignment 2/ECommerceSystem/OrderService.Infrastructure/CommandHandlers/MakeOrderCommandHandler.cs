using System.Text;
using System.Text.Json;
using Common.Domain;
using Common.Domain.Bus;
using Common.Domain.DTO;
using Common.Domain.Events;
using Common.Domain.Events.OrderService;
using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Commands;
using OrderService.Domain.Entities;
using OrderService.Infrastructure.Repositories;

namespace OrderService.Infrastructure.CommandHandlers;

public class MakeOrderCommandHandler : IRequestHandler<MakeOrderCommand, bool>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IEventBus _eventBus;
    private readonly HttpClient _httpClient;
    private readonly ILogger<MakeOrderCommandHandler> _logger;

    public MakeOrderCommandHandler(IOrderRepository orderRepository, IOrderItemRepository orderItemRepository, IEventBus eventBus, HttpClient httpClient, ILogger<MakeOrderCommandHandler> logger)
    {
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _eventBus = eventBus;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<bool> Handle(MakeOrderCommand request, CancellationToken cancellationToken)
    {
        _logger.LogWarning("MakeOrderCommand");

        var json = JsonSerializer.Serialize(new { token = request.Token });
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync($"{Constants.USER_SERVICE_ADDRESS}/authorize", content);

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        response = await _httpClient.GetAsync($"{Constants.CART_SERVICE_ADDRESS}/get-user-cart-items/{request.UserId}");

        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var items = JsonSerializer.Deserialize<List<CartEntryDto>>(json, options);

        if (items == null || items.Count == 0)
        {
            return false;
        }

        var newOrder = new Order(request.UserId);
        await _orderRepository.CreateOrder(newOrder);

        List<ItemQuantityDto> itemQuantities = items.Select(item => new ItemQuantityDto(item.ProductId, item.Quantity)).ToList();

        foreach (var orderItem in itemQuantities)
        {
            var newOrderItem = new OrderItem(newOrder.Id, orderItem.ProductId, orderItem.Quantity);
            await _orderItemRepository.CreateOrderItem(newOrderItem);
        }

        await _eventBus.Publish(new OrderCreatedEvent(newOrder.UserId, newOrder.Id, itemQuantities));
        return true;
    }
}