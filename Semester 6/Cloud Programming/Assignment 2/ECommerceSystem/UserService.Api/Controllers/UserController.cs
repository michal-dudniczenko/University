using Microsoft.AspNetCore.Mvc;
using UserService.Domain.DTO;
using MediatR;
using UserService.Infrastructure.Helpers;
using UserService.Domain.Commands;
using UserService.Domain.Queries;

namespace UserService.Api.Controllers;

[Route("")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserController> _logger;
    private readonly ITokenService _tokenService;

    public UserController(IMediator mediator, ILogger<UserController> logger, ITokenService tokenService)
    {
        _mediator = mediator;
        _logger = logger;
        _tokenService = tokenService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto request)
    {
        _logger.LogWarning("POST /register");

        var result = await _mediator.Send(new RegisterUserCommand(request.Email, request.Password));

        if (result)
        {
            return Ok();
        }
        else
        {
            return Conflict(new { error = "User with that email already exists." });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request)
    {
        _logger.LogWarning("POST /login");

        var result = await _mediator.Send(new LoginUserCommand(request.Email, request.Password));

        if (!result)
        {
            return Unauthorized(new { error = "Invalid email or password." });
            
        }

        var token = await _tokenService.GenerateToken(request.Email);
        return Ok(new { token = token });
    }

    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize([FromBody] AuthorizeUserDto request)
    {
        _logger.LogWarning("POST /authorize");

        var result = await _mediator.Send(new AuthorizeUserCommand(request.Token));

        if ( result)
        {
            return Ok();
        } else
        {
            return Unauthorized(new { error = "Invalid token." });
        }
    }

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        _logger.LogWarning("GET /get-all-users");

        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }

    [HttpGet("get-user-email/{id}")]
    public async Task<IActionResult> GetUserEmail(Guid id)
    {
        _logger.LogWarning($"GET /get-user-email/{id}");

        var result = await _mediator.Send(new GetUserEmailQuery(id));
        if (result == null)
        {
            return NotFound();
        }
        return Ok(result);
    }
}
