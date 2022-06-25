using DashboardDomain.Features.CQRS.Command;
using DashboardDomain.Features.CQRS.Query;
using DashboardDomain.Features.DTO.Query;
using Flaminco.CommitResult;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DashboardService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SectionGroupController : ControllerBase
{
    private readonly IMediator _mediator;
    public SectionGroupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> InsertSectionGroup([FromBody] string Name, CancellationToken token)
         => Ok(await _mediator.Send(new InsertSectionGroupCommand(Name), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> UpdateSectionGroup([FromQuery] int Id, [FromQuery] string Name, CancellationToken token)
         => Ok(await _mediator.Send(new UpdateSectionGroupCommand(Id, Name), token));

    [HttpDelete("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> DeleteSectionGroup([FromQuery] int Id, CancellationToken token)
         => Ok(await _mediator.Send(new DeleteSectionGroupCommand(Id), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<SectionGroupResponse>)), AllowAnonymous]
    public async Task<IActionResult> GetAllGroupSection(CancellationToken token)
         => Ok(await _mediator.Send(new GetAllGroupSectionQuery(), token));
}