using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.Invitations.CQRS.DTO.Query;
using NotifierDomain.Features.Invitations.CQRS.Query;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using NotifierInfrastructure.HttpClients;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.Invitations.CQRS.Query;
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
        // Get List Of Identity Users
        // We Will remove Invitation Request
        CommitResults<IdentityUserInvitationResponse>? identityUserInvitationResponses = await _IdentityClient.GetIdentityUserInvitationsAsync(invitations.Select(a => a.InviterId), cancellationToken);


        IEnumerable<InvitationResponse> Mapper()
        {
            foreach (Invitation invitation in invitations)
            {
                yield return new InvitationResponse
                {
                    CreatedOn = invitation.CreatedOn.GetValueOrDefault(),
                    InvitationId = invitation.Id,
                    IsSeen = invitation.IsSeen,
                    Status = (int)invitation.Status,
                    Argument = invitation.Argument,
                    Description = $"{invitation.InvitationTypeFK.Name} {identityUserInvitationResponses.Value.SingleOrDefault(a => a.Id.Equals(invitation.InviterId)).FullName} {invitation.InvitationTypeFK.Description}",
                    AvatarUrl = identityUserInvitationResponses.Value.SingleOrDefault(a => a.Id.Equals(invitation.InviterId)).Avatar
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
