using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityDomain.Features.IdentityUserTransaction.CQRS.Command;
using IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;
using IdentityDomain.Features.IdentityUserTransaction.DTO;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;
using SharedModule.DTO;

namespace IdentityService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ParentController : ControllerBase
{
    private readonly IMediator _mediator;

    public ParentController(IMediator mediator)
    {
        _mediator = mediator;
    }
    [HttpPost("[action]"), Produces(typeof(CommitResult<AddNewChildResponse>))]
    public async Task<IActionResult> AddNewChild([FromBody] AddNewChildRequest AddNewChildRequest, CancellationToken token)
        => Ok(await _mediator.Send(new AddNewChildCommand(AddNewChildRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> RemoveChild([FromBody] RemoveChildRequest RemoveChildRequest, CancellationToken token)
        => Ok(await _mediator.Send(new RemoveChildCommand(RemoveChildRequest), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<LimitedProfileResponse>))]
    public async Task<IActionResult> GetIdentityLimitedProfileByEmailOrMobile([FromQuery(Name = "Email")] string? Email, [FromQuery(Name = "MobileNumber")] string? MobileNumber, CancellationToken token)
         => Ok(await _mediator.Send(new GetIdentityLimitedProfileByEmailOrMobileQuery(Email, MobileNumber), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<LimitedProfileResponse>))]
    public async Task<IActionResult> GetParentKids(CancellationToken token)
         => Ok(await _mediator.Send(new GetIdentityRelationUserQuery(), token));
}
