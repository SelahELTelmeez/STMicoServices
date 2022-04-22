using MediatR;
using Microsoft.AspNetCore.Mvc;
using NotifierDomain.Features.Invitations.CQRS.Command;
using NotifierDomain.Features.Invitations.CQRS.DTO.Command;
using NotifierDomain.Features.Invitations.CQRS.Query;
using NotifierDomain.Features.Notification.CQRS.Command;
using NotifierDomain.Features.Notification.CQRS.Query;
using NotifierDomain.Features.Notification.DTO.Command;

namespace NotifierService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class NotificationController : ControllerBase
{
    private readonly IMediator _mediator;

    public NotificationController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SendNotification([FromBody] NotificationRequest NotificationRequest, CancellationToken token)
        => Ok(await _mediator.Send(new CreateNotificationCommand(NotificationRequest), token));

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllNotifications(CancellationToken token)
        => Ok(await _mediator.Send(new GetAllNotificationsQuery(), token));

    [HttpGet("[action]")]
    public async Task<IActionResult> GetIdentityInvitations(CancellationToken token)
        => Ok(await _mediator.Send(new GetInvitationsQuery(), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> SendInvitation([FromBody] InvitationRequest InvitationRequest, CancellationToken token)
        => Ok(await _mediator.Send(new CreateInvitationCommand(InvitationRequest), token));

    [HttpPost("[action]")]
    public async Task<IActionResult> SetAsInactiveInvitation([FromBody] int InvitationId, CancellationToken token)
        => Ok(await _mediator.Send(new SetAsInactiveInvitationCommand(InvitationId), token));
}