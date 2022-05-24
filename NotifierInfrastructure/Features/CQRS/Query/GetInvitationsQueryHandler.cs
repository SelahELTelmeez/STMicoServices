using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.DTO.Query;
using NotifierDomain.Features.CQRS.Query;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using NotifierInfrastructure.HttpClients;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Query;
public class GetInvitationsQueryHandler : IRequestHandler<GetInvitationsQuery, CommitResults<InvitationResponse>>
{
    private readonly NotifierDbContext _dbContext;
    private readonly IdentityClient _IdentityClient;
    private readonly Guid? _userId;

    public GetInvitationsQueryHandler(IdentityClient identityClient, IHttpContextAccessor httpContextAccessor, NotifierDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _IdentityClient = identityClient;
    }
    public async Task<CommitResults<InvitationResponse>> Handle(GetInvitationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Invitation> invitations = await _dbContext.Set<Invitation>()
                                                              .Where(a => a.InvitedId.Equals(_userId))
                                                              .Include(a => a.InvitationTypeFK)
                                                              .OrderByDescending(a => a.CreatedOn)
                                                              .ToListAsync(cancellationToken);

        if (!invitations.Any())
        {
            return new CommitResults<InvitationResponse>
            {
                ResultType = ResultType.Empty,
                Value = Array.Empty<InvitationResponse>()
            };
        }

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _IdentityClient.GetLimitedProfilesAsync(invitations.Select(a => a.InviterId), cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return new CommitResults<InvitationResponse>
            {
                ErrorCode = limitedProfiles.ErrorCode,
                ErrorMessage = limitedProfiles.ErrorMessage,
                ResultType = limitedProfiles.ResultType
            };
        }

        IEnumerable<InvitationResponse> Mapper()
        {
            foreach (Invitation invitation in invitations)
            {
                LimitedProfileResponse limitedProfile = limitedProfiles.Value.Single(a => a.UserId == invitation.InviterId);
                yield return new InvitationResponse
                {
                    CreatedOn = invitation.CreatedOn.GetValueOrDefault(),
                    InvitationId = invitation.Id,
                    IsSeen = invitation.IsSeen,
                    Status = (int)invitation.Status,
                    Argument = invitation.Argument,
                    Description = invitation.Message,
                    AvatarUrl = limitedProfile.AvatarImage,
                    InvitedId = invitation.InvitedId,
                    InviterId = invitation.InviterId,
                    Type = invitation.InvitationTypeId
                };
            }
            yield break;
        };
        return new CommitResults<InvitationResponse>()
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };
    }
}
