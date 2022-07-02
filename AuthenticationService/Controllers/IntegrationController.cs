using IdentityDomain.Features.Integration.CQRS.Command;
using IdentityDomain.Features.Integration.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityService.Controllers;

[ApiController, Route("api/[controller]")]
public class IntegrationController : ControllerBase
{
    private readonly IMediator _mediator;

    public IntegrationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("[action]"), AllowAnonymous]
    public async Task<IActionResult> Verify([FromQuery(Name = "ExternalUserId")] string externalUserId, [FromQuery(Name = "provider")] string provider, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new VerifyExternalUserCommand(externalUserId, provider), cancellationToken));
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register([FromBody] ExternalUserRegisterRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new RegisterExternalUserCommand(request), cancellationToken));
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> RegisterAsBulk(IFormFile file, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new RegisterExternalUserAsBulkCommand(file.OpenReadStream()), cancellationToken));
    }


}
