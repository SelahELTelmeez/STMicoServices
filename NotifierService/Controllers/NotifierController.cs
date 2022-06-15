using Flaminco.CommitResult;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotifierDomain.Features.CQRS.Command;
using NotifierDomain.Features.CQRS.DTO.Query;
using NotifierDomain.Features.CQRS.Query;
using NotifierDomain.Features.DTO.Command;
using SharedModule.DTO;

namespace NotifierService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotifierController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotifierController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]"), Produces(typeof(ICommitResult))]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest NotificationRequest, CancellationToken token)
        => Ok(await _mediator.Send(new SendNotificationCommand(NotificationRequest), token));

    [HttpGet("[action]"), Produces(typeof(ICommitResults<NotificationResponse>))]
    public async Task<IActionResult> GetNotifications(CancellationToken token)
        => Ok(await _mediator.Send(new GetNotificationsQuery(), token));

    [HttpGet("[action]"), Produces(typeof(ICommitResults<InvitationResponse>))]
    public async Task<IActionResult> GetInvitations(CancellationToken token)
        => Ok(await _mediator.Send(new GetInvitationsQuery(), token));

    [HttpPost("[action]"), Produces(typeof(ICommitResult))]
    public async Task<IActionResult> SendInvitation([FromBody] InvitationRequest InvitationRequest, CancellationToken token)
        => Ok(await _mediator.Send(new SendInvitationCommand(InvitationRequest), token));

    [HttpGet("[action]"), Produces(typeof(ICommitResult))]
    public async Task<IActionResult> SetAsInActive([FromQuery(Name = "InvitationId")] int InvitationId, [FromQuery(Name ="Status") ] int Status, CancellationToken token)
        => Ok(await _mediator.Send(new SetAsInactiveInvitationCommand(InvitationId, Status), token));

    [HttpPost("[action]"), Produces(typeof(ICommitResults<ClassStatusResponse>))]
    public async Task<IActionResult> GetClassesCurrentStatus([FromBody] IEnumerable<int> ClassIds, CancellationToken token)
        => Ok(await _mediator.Send(new GetClassesCurrentStatusQuery(ClassIds), token));
}