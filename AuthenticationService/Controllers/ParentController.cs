using IdentityDomain.Features.IdentityUserTransaction.CQRS.Command;
using IdentityDomain.Features.IdentityUserTransaction.CQRS.Query;
using IdentityDomain.Features.IdentityUserTransaction.DTO;
using IdentityDomain.Features.Shared.DTO;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;

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

    [HttpPost("[action]"), Produces(typeof(CommitResult<SearchOnStudentResponse>))]
    public async Task<IActionResult> SearchOnStudent([FromBody] SearchOnStudentRequest SearchOnStudentRequest, CancellationToken token)
         => Ok(await _mediator.Send(new SearchOnStudentQuery(SearchOnStudentRequest), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<LimitedProfileResponse>))]
    public async Task<IActionResult> GetParentKids(CancellationToken token)
         => Ok(await _mediator.Send(new GetIdentityRelationUserQuery(), token));
}
