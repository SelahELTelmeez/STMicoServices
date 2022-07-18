using ChatDomain.Features.CQRS.Command;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace ChatService.Hubs;

[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
public class ChatHub : Hub
{
    private readonly IMediator _mediator;
    public ChatHub(IMediator mediator)
    {
        _mediator = mediator;
    }
    // Invoked Directly once a user gets connected
    public override async Task OnConnectedAsync()
    {
        await _mediator.Send(new AddChatSessionCommand(Context.ConnectionId));
    }
    //// Invoked Directly once a user gets Disconnected
    public override async Task OnDisconnectedAsync(Exception exception)
    {
        await _mediator.Send(new RemoveChatSessionCommand(Context.ConnectionId));
    }
}
