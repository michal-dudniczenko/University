using CartService.Domain.Commands;
using CartService.Domain.DTO;
using CartService.Domain.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CartService.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<CartController> _logger;

    public CartController(IMediator mediator, ILogger<CartController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpGet("get-user-cart-items/{userId}")]
    public async Task<ActionResult<List<CartEntryDto>>> GetUserCartItems(Guid userId)
    {
        var result = await _mediator.Send(new GetUserCartItemsQuery(userId));
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPost("add-to-cart/{productId}/{userId}")]
    public async Task<IActionResult> AddToCart(Guid productId, Guid userId)
    {
        var result = await _mediator.Send(new AddItemToCartCommand(productId, userId));
        if (result)
        {
            return Ok();
        }
        else
        {
            return Conflict();
        }
    }

    [HttpPost("remove-from-cart/{productId}/{userId}")]
    public async Task<IActionResult> RemoveFromCart(Guid productId, Guid userId)
    {
        var result = await _mediator.Send(new RemoveItemFromCartCommand(productId, userId));
        if (result)
        {
            return Ok();
        }
        else
        {
            return Conflict();
        }
    }

    [HttpPost("clear-user-cart/{userId}")]
    public async Task<IActionResult> ClearUserCart(Guid userId)
    {
        var result = await _mediator.Send(new ClearUserCartCommand(userId));
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
