using IdentityDomain.Features.IdentityLimitedProfile.CQRS.Query;
using IdentityDomain.Features.Parent.CQRS.Command;
using IdentityDomain.Features.Parent.CQRS.Query;
using IdentityDomain.Features.Parent.DTO;
using IdentityInfrastructure.Features.Parent.CQRS.Command;
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
    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> AddChildAccount([FromBody] AddNewChildRequest AddNewChildRequest, CancellationToken token)
        => Ok(await _mediator.Send(new AddChildAccountCommand(AddNewChildRequest), token));


    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> AcceptAddChildInvitation([FromBody] AddChildInvitationRequest AddChildInvitationRequest, CancellationToken token)
     => Ok(await _mediator.Send(new AcceptChildInvitationRequestCommand(AddChildInvitationRequest), token));


    [HttpGet("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> RequestAddChildIvitation([FromQuery(Name = "ChildId")] Guid ChildId, CancellationToken token)
     => Ok(await _mediator.Send(new RequestToAddChildCommand(ChildId), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> RemoveChild([FromQuery(Name = "ChildId")] Guid ChildId, CancellationToken token)
        => Ok(await _mediator.Send(new RemoveChildCommand(ChildId), token));


    [HttpGet("[action]"), Produces(typeof(CommitResult<LimitedProfileResponse>))]
    public async Task<IActionResult> GetIdentityLimitedProfileByEmailOrMobile([FromQuery(Name = "Email")] string? Email, [FromQuery(Name = "MobileNumber")] string? MobileNumber, CancellationToken token)
         => Ok(await _mediator.Send(new GetIdentityLimitedProfileByEmailOrMobileQuery(Email, MobileNumber), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<LimitedProfileResponse>))]
    public async Task<IActionResult> GetAssociatedChildren(CancellationToken token)
         => Ok(await _mediator.Send(new GetAssociatedChildrenQuery(), token));
}
