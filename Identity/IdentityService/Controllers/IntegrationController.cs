using IdentityDomain.Features.Integration.CQRS.Command;
using IdentityDomain.Features.Integration.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;

namespace IdentityService.Controllers;

[ApiController, Route("api/[controller]")]
public class IntegrationController : ControllerBase
{
    private readonly IMediator _mediator;

    public IntegrationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("[action]"), Produces(typeof(CommitResult<string>))]
    public async Task<IActionResult> Verify([FromQuery(Name = "Key")] Guid Key, [FromQuery(Name = "ExternalUserId")] string externalUserId, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new VerifyExternalUserCommand(externalUserId, Key), cancellationToken));
    }

    [HttpPost("[action]"), Produces(typeof(CommitResult<string>))]
    public async Task<IActionResult> Register([FromQuery(Name = "Key")] Guid Key, [FromBody] ExternalUserRegisterRequest request, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new RegisterExternalUserCommand(request, Key), cancellationToken));
    }

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> RegisterAsBulk([FromQuery(Name = "Key")] Guid Key, IFormFile file, CancellationToken cancellationToken)
    {
        return Ok(await _mediator.Send(new RegisterExternalUserAsBulkCommand(file.OpenReadStream(), Key), cancellationToken));
    }


}
