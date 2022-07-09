using ChatDomain.Features.CQRS.Query;
using ChatDomain.Features.DTO;
using ChatEntities;
using ChatEntities.Entities;
using ChatInfrastructure.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace ChatInfrastructure.Features.CQRS.Query;

public class GetOnlineUsersHandler : IRequestHandler<GetOnlineUsersQuery, ICommitResults<OnlineUser>>
{
    private readonly ChatDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly IdentityClient _identityClient;
    public GetOnlineUsersHandler(ChatDbContext dbContext, IHttpContextAccessor httpContextAccessor, IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _identityClient = identityClient;
    }
    public async Task<ICommitResults<OnlineUser>> Handle(GetOnlineUsersQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ChatSession> chatSessions = await _dbContext.Set<ChatSession>()
                                                                .Where(a => !a.UserId.Equals(_userId))
                                                                .ToListAsync(cancellationToken);
        if (!chatSessions.Any())
        {
            return ResultType.Ok.GetValueCommitResults(Array.Empty<OnlineUser>());
        }

        ICommitResults<LimitedProfileResponse>? profileResponses = await _identityClient.GetLimitedProfilesAsync(chatSessions.Select(a => a.UserId), cancellationToken);

        if (!profileResponses?.IsSuccess ?? true)
        {
            return profileResponses.ResultType.GetValueCommitResults(Array.Empty<OnlineUser>(), profileResponses.ErrorCode, profileResponses.ErrorMessage);
        }

        IEnumerable<OnlineUser> Mapper()
        {
            foreach (var session in chatSessions)
            {
                LimitedProfileResponse? limitedProfile = profileResponses?.Value?.FirstOrDefault(a => a.UserId == session.UserId);
                if (limitedProfile != null)
                {
                    yield return new OnlineUser
                    {
                        UserId = limitedProfile.UserId,
                        AvatarImage = limitedProfile.AvatarImage,
                        ConnectionId = session.ConnectionId,
                        UserName = limitedProfile.FullName
                    };
                }
            }
        }

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}
