using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotifierDomain.Features.Invitations.CQRS.Command;
using NotifierDomain.Features.Invitations.CQRS.DTO.Command;
using NotifierDomain.Features.Invitations.CQRS.Query;
using NotifierDomain.Features.Notification.CQRS.Command;
using NotifierDomain.Features.Notification.CQRS.Query;
using NotifierDomain.Features.Notification.DTO.Command;

namespace NotifierService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class NotifierController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotifierController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest NotificationRequest, CancellationToken token)
        => Ok(await _mediator.Send(new SendNotificationCommand(NotificationRequest), token));

    [HttpGet("[action]")]
    public async Task<IActionResult> GetNotifications(CancellationToken token)
        => Ok(await _mediator.Send(new GetNotificationsQuery(), token));

    [HttpGet("[action]")]
    public async Task<IActionResult> GetInvitations(CancellationToken token)
        => Ok(await _mediator.Send(new GetInvitationsQuery(), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> SendInvitation([FromBody] InvitationRequest InvitationRequest, CancellationToken token)
        => Ok(await _mediator.Send(new SendInvitationCommand(InvitationRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> SetAsInactiveInvitation([FromBody] int InvitationId, CancellationToken token)
        => Ok(await _mediator.Send(new SetAsInactiveInvitationCommand(InvitationId), token));
}