using DashboardDomain.Features.CQRS.Command;
using DashboardDomain.Features.CQRS.Query;
using DashboardDomain.Features.DTO.Command;
using DashboardDomain.Features.DTO.Query;
using DashboardEntity.Entities;
using Flaminco.CommitResult;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DashboardService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class AppSettingController : ControllerBase
{
    private readonly IMediator _mediator;
    public AppSettingController(IMediator mediator, DashboardDbContext dashboardDbContext)
    {
        _mediator = mediator;
        dashboardDbContext.Database.EnsureCreated();
    }

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> CreateAppSetting([FromBody] InsertAppSettingRequest InsertAppSettingRequest, CancellationToken token)
         => Ok(await _mediator.Send(new InsertAppSettingCommand(InsertAppSettingRequest), token));

    [HttpPost("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> UpdateAppSetting([FromBody] UpdateAppSettingRequest UpdateAppSettingRequest, CancellationToken token)
         => Ok(await _mediator.Send(new UpdateAppSettingCommand(UpdateAppSettingRequest), token));

    [HttpDelete("[action]"), Produces(typeof(CommitResult))]
    public async Task<IActionResult> DeleteAppSetting([FromQuery] int Id, CancellationToken token)
         => Ok(await _mediator.Send(new DeleteAppSettingCommand(Id), token));

    [HttpGet("[action]"), Produces(typeof(CommitResults<AppSettingResponse>)), AllowAnonymous]
    public async Task<IActionResult> GetAllAppSettings(CancellationToken token)
         => Ok(await _mediator.Send(new GetAllAppSettingsQuery(), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<AppSettingResponse>)), AllowAnonymous]
    public async Task<IActionResult> GetAppSettingById([FromQuery] int Id, CancellationToken token)
         => Ok(await _mediator.Send(new GetAppSettingByIdQuery(Id), token));

    [HttpGet("[action]"), Produces(typeof(CommitResult<AppSettingResponse>)), AllowAnonymous]
    public async Task<IActionResult> GetAppSettingByName([FromQuery] string Name, CancellationToken token)
         => Ok(await _mediator.Send(new GetAppSettingByNameQuery(Name), token));
}
