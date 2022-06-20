using DashboardDomain.Features.CQRS.Command;
using DashboardDomain.Features.CQRS.Query;
using DashboardDomain.Features.DTO.Command;
using DashboardDomain.Features.DTO.Query;
using Flaminco.CommitResult;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DashboardService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class SectionController : ControllerBase
{
    private readonly IMediator _mediator;
    public SectionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> InsertSection([FromBody] InsertSectionRequest InsertSectionRequest, CancellationToken token)
         => Ok(await _mediator.Send(new InsertSectionCommand(InsertSectionRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> UpdateSection([FromBody] UpdateSectionRequest UpdateSectionRequest, CancellationToken token)
         => Ok(await _mediator.Send(new UpdateSectionCommand(UpdateSectionRequest), token));

    [HttpDelete("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> DeleteSection([FromQuery] int Id, CancellationToken token)
         => Ok(await _mediator.Send(new DeleteSectionCommand(Id), token));

    [HttpDelete("[action]"), Produces(typeof(CommitResults<SectionResponse>))]
    public async Task<IActionResult> GetSectionsByGroupId([FromQuery] int GroupId, CancellationToken token)
         => Ok(await _mediator.Send(new GetSectionsByGroupIdQuery(GroupId), token));
}