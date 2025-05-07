using Microsoft.AspNetCore.Mvc;
using UserService.Domain.DTO;
using MediatR;
using UserService.Infrastructure.Helpers;
using UserService.Domain.Commands;
using UserService.Domain.Queries;

namespace UserService.Api.Controllers;

[Route("[controller]")]
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
        var result = await _mediator.Send(new RegisterUserCommand(request.Email, request.Password));

        if (result)
        {
            return Ok();
        }
        else
        {
            return Conflict("User with that email already exists.");
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto request)
    {
        var result = await _mediator.Send(new LoginUserCommand(request.Email, request.Password));

        if (!result)
        {
            return Unauthorized("Invalid email or password.");
            
        }

        var token = await _tokenService.GenerateToken(request.Email);
        return Ok(token);
    }

    [HttpPost("authorize")]
    public async Task<IActionResult> Authorize([FromBody] AuthorizeUserDto request)
    {
        var result = await _mediator.Send(new AuthorizeUserCommand(request.Token));

        if ( result)
        {
            return Ok();
        } else
        {
            return Unauthorized("Invalid token.");
        }
    }

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _mediator.Send(new GetAllUsersQuery());
        return Ok(result);
    }
}
