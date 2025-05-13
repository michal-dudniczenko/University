using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Domain.Commands;
using ProductService.Domain.DTO;
using ProductService.Domain.Queries;

namespace ProductService.Api.Controllers;

[ApiController]
[Route("")]
public class ProductController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IMediator mediator, ILogger<ProductController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductDto request)
    {
        _logger.LogWarning("POST /create");

        var result = await _mediator.Send(new CreateProductCommand(request.Name, request.Description, request.Price, request.Quantity));
        if (result)
        {
            return Ok();
        }
        else
        {
            return Conflict();
        }
    }

    [HttpGet("get/{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(Guid id)
    {
        _logger.LogWarning("GET /get/{id}", id);

        var result = await _mediator.Send(new GetProductByIdQuery(id));
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("get-all")]
    public async Task<ActionResult<List<ProductDto>>> GetAllProducts()
    {
        _logger.LogWarning("GET /get-all");

        var result = await _mediator.Send(new GetAllProductsQuery());
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpGet("get-available")]
    public async Task<ActionResult<List<ProductDto>>> GetAvailableProducts()
    {
        _logger.LogWarning("GET /get-available");

        var result = await _mediator.Send(new GetAvailableProductsQuery());
        if (result != null)
        {
            return Ok(result);
        }
        else
        {
            return NotFound();
        }
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateProduct([FromBody] ProductDto request)
    {
        _logger.LogWarning("PUT /update");

        var result = await _mediator.Send(new UpdateProductCommand(request.Id, request.Name, request.Description, request.Price, request.Quantity));
        if (result)
        {
            return Ok();
        }
        else
        {
            return Conflict();
        }
    }

    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteProduct(Guid id)
    {
        _logger.LogWarning("DELETE /delete/{id}", id);

        var result = await _mediator.Send(new DeleteProductCommand(id));
        if (result)
        {
            return Ok();
        }
        else
        {
            return NotFound();
        }
    }

}
