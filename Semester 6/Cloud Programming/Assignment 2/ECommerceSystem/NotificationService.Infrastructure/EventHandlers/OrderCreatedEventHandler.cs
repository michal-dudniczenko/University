using System.Text.Json;
using Common.Domain.Bus;
using Common.Domain.Events.OrderService;
using MediatR;
using NotificationService.Domain.Commands;
using Microsoft.Extensions.Logging;
using Common.Domain.DTO;


namespace NotificationService.Infrastracture.EventHandlers;

public class OrderCreatedEventHandler : IEventHandler<OrderCreatedEvent>
{
    private readonly IMediator _mediator;
    private readonly HttpClient _httpClient;
    private readonly ILogger<OrderCreatedEventHandler> _logger;

    public OrderCreatedEventHandler(IMediator mediator, HttpClient httpClient, ILogger<OrderCreatedEventHandler> logger)
    {
        _mediator = mediator;
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task Handle(OrderCreatedEvent @event)
    {
        _logger.LogWarning("OrderCreatedEventHandler");

        var response = await _httpClient.GetAsync($"https://localhost:50001/get-user-email/{@event.UserId}");
        
        if (!response.IsSuccessStatusCode)
        {
            return;
        }

        var json = await response.Content.ReadAsStringAsync();

        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var userEmail = JsonSerializer.Deserialize<UserEmailDto>(json, options).Email;

        if (userEmail == null)
        {
            return;
        }

        await _mediator.Send(new SendOrderNotificationCommand(@event.UserId, @event.OrderId, userEmail));
    }
}