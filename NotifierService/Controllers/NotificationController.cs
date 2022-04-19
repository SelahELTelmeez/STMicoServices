using MediatR;
using Microsoft.AspNetCore.Mvc;
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
    public async Task<IActionResult> CreateNotification([FromBody] NotificationRequest NotificationRequest, CancellationToken token)
        => Ok(await _mediator.Send(new CreateNotificationCommand(NotificationRequest), token));

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllNotifications(CancellationToken token)
           => Ok(await _mediator.Send(new GetAllNotificationsQuery(), token));

    [HttpGet("[action]")]
    public async Task<IActionResult> GetIdentityInvitations(CancellationToken token)
      => Ok(await _mediator.Send(new GetIdentityInvitationsQuery(), token));
}