using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Commands;
using OrderService.Domain.DTO;
using OrderService.Domain.Queries;

namespace OrderService.Api.Controllers;

[Route("")]
[ApiController]
public class OrderController : ControllerBase
{
    public readonly IMediator _mediator;
    public readonly ILogger<OrderController> _logger;

    public OrderController(IMediator mediator, ILogger<OrderController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("get-user-orders/{userId}")]
    public async Task<ActionResult<List<OrderDto>>> GetUserOrders(Guid userId)
    {
        _logger.LogWarning("GET /get-user-orders/{userId}", userId);

        var result = await _mediator.Send(new GetUserOrdersQuery(userId));
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("get-order-items/{orderId}")]
    public async Task<ActionResult<List<OrderItemDto>>> GetOrderItems(Guid orderId)
    {
        _logger.LogWarning("GET /get-order-items/{orderId}", orderId);

        var result = await _mediator.Send(new GetOrderItemsQuery(orderId));
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost("make-order")]
    public async Task<IActionResult> MakeOrder([FromBody] MakeOrderDto makeOrderDto)
    {
        _logger.LogWarning("POST /make-order");

        var result = await _mediator.Send(new MakeOrderCommand(userId: makeOrderDto.UserId, token: makeOrderDto.Token));
        if (result)
        {
            return Ok();
        }
        else
        {
            return Conflict();
        }
    }
}
