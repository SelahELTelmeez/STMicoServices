using ChatDomain.Features.CQRS.Command;
using ChatDomain.Features.CQRS.Query;
using ChatDomain.Features.DTO;
using ChatService.Hubs;
using Flaminco.CommitResult;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Controllers;

[ApiController, Route("api/[controller]"), Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatController : ControllerBase
{
    private readonly IMediator _mediator;
    private IHubContext<ChatHub> _hub;
    public ChatController(IMediator mediator, IHubContext<ChatHub> hub)
    {
        _mediator = mediator;
        _hub = hub;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetOnlineUsers(CancellationToken token)
    {
        return Ok(await _mediator.Send(new GetOnlineUsersQuery(), token));
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> SendMessage([FromBody] ServerMessage serverMessage, CancellationToken token)
    {
        ICommitResult AddCommitResult = await _mediator.Send(new AddChatMessageCommand(serverMessage), token);

        if (AddCommitResult.IsSuccess)
        {
            ICommitResult<string> commitResult = await _mediator.Send(new GetConnectionIdByUserIdQuery(serverMessage.ToId), token);

            if (commitResult.IsSuccess)
            {
                await _hub.Clients.Client(commitResult.Value).SendAsync("ReceiveMessage", serverMessage);
            }
            return Ok(commitResult);

        }
        else
        {
            // Coudn't send the message
            return Ok(AddCommitResult);
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> LoadMessages([FromQuery(Name = "PeerId")] Guid PeerId, CancellationToken token)
    {
        return Ok(await _mediator.Send(new LoadMessagesP2PQuery(PeerId), token));
    }
}
