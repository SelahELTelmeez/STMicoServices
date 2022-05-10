using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityDomain.Features.IdentityAvatars.CQRS.Query;
using IdentityDomain.Features.IdentityAvatars.DTO.Query;
using IdentityDomain.Features.IdentityGovernorates.CQRS.Query;
using IdentityDomain.Features.IdentityGovernorates.DTO;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ResultHandler;
using SharedModule.DTO;

namespace IdentityService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class LookupController : ControllerBase
{
    private readonly IMediator _mediator;

    public LookupController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("[action]"), Produces(typeof(CommitResults<IdentityAvatarResponse>))]
    public async Task<IActionResult> GetAvatars(CancellationToken token)
        => Ok(await _mediator.Send(new GetIdentityAvatarsQuery(), token));


    [HttpGet("[action]"), AllowAnonymous, Produces(typeof(CommitResults<IdentityGovernorateResponse>))]
    public async Task<IActionResult> GetGovernorates(CancellationToken token)
        => Ok(await _mediator.Send(new GetIdentityGovernoratesQuery(), token));


    [HttpGet("[action]"), AllowAnonymous, Produces(typeof(CommitResults<GradeResponse>))]
    public async Task<IActionResult> GetGrades(CancellationToken token)
         => Ok(await _mediator.Send(new GetGradesQuery(), token));

    [HttpGet("[action]"), AllowAnonymous, Produces(typeof(CommitResult<GradeResponse>))]
    public async Task<IActionResult> GetGradeById([FromQuery(Name = "GradeId")] int GradeId, CancellationToken token)
         => Ok(await _mediator.Send(new GetGradeByIdQuery(GradeId), token));

    [HttpPost("[action]"), AllowAnonymous, Produces(typeof(CommitResults<GradeResponse>))]
    public async Task<IActionResult> GetGradeByIds([FromBody] IEnumerable<int> GradeIds, CancellationToken token)
         => Ok(await _mediator.Send(new GetGradeByIdsQuery(GradeIds), token));

}
