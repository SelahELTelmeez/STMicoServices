using MediatR;
using Microsoft.AspNetCore.Mvc;
using TransactionDomain.Features.Notification.CQRS.Command;
using TransactionDomain.Features.Notification.CQRS.Query;
using TransactionDomain.Features.Notification.DTO.Command;

namespace TransactionService.Controllers
{
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
    }
}
