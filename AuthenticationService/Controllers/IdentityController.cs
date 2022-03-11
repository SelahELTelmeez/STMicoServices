using IdentityDomain.Features.Login.CQRS.Command;
using IdentityDomain.Features.Login.DTO.Command;
using IdentityDomain.Features.Refresh.CQRS.Command;
using IdentityDomain.Features.Register.CQRS.Command;
using IdentityDomain.Features.Register.DTO.Command;
using MediatR;
using Microsoft.AspNetCore.Mvc;
namespace IdentityService.Controllers;
[ApiController, Route("api/[controller]")]
public class IdentityController : ControllerBase
{
    private readonly IMediator _mediator;
    public IdentityController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest, CancellationToken token)
        => Ok(await _mediator.Send(new LoginCommand(loginRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO registerRequest, CancellationToken token)
         => Ok(await _mediator.Send(new RegisterCommand(registerRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> RefreshToken([FromBody] string refreshToken, CancellationToken token)
         => Ok(await _mediator.Send(new RefreshTokenCommand(refreshToken), token));
}