using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Invitations.CQRS.DTO.Query;
using TransactionDomain.Features.Invitations.CQRS.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.HttpClients;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.Invitation;

namespace TransactionInfrastructure.Features.Invitations.CQRS.Query;
public class GetIdentityInvitationsQueryHandler : IRequestHandler<GetIdentityInvitationsQuery, CommitResults<IdentityInvitationResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly IdentityClient _IdentityClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetIdentityInvitationsQueryHandler(IdentityClient identityClient, IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _IdentityClient = identityClient;
    }
    public async Task<CommitResults<IdentityInvitationResponse>> Handle(GetIdentityInvitationsQuery request, CancellationToken cancellationToken)
    {
        List<DomainEntities.Invitation> invitations = await _dbContext.Set<DomainEntities.Invitation>()
                                                                      .Where(a => a.InvitedId.Equals(_httpContextAccessor.GetIdentityUserId()))
                                                                      .Include(a => a.InvitationTypeFK)
                                                                      .OrderByDescending(a => a.CreatedOn).ToListAsync(cancellationToken);
        // Get List Of Identity Users
        // We Will remove Invitation Request
        CommitResults<IdentityUserInvitationResponse>? identityUserInvitationResponses = await _IdentityClient.GetIdentityUserInvitationsAsync(invitations.Select(a => a.InviterId), cancellationToken);


        IEnumerable<IdentityInvitationResponse> InvitationMapper()
        {
            foreach (DomainEntities.Invitation invitation in invitations)
            {
                yield return new IdentityInvitationResponse
                {
                    CreatedOn = invitation.CreatedOn.GetValueOrDefault(),
                    InvitationId = invitation.Id,
                    IsNew = invitation.IsNew,
                    IsSeen = invitation.IsSeen,
                    Status = (int)invitation.Status,
                    Description = $"{invitation.InvitationTypeFK.Name} {identityUserInvitationResponses.Value.SingleOrDefault(a => a.Id.Equals(invitation.InviterId)).FullName} {invitation.InvitationTypeFK.Description}",
                    AvatarUrl = identityUserInvitationResponses.Value.SingleOrDefault(a => a.Id.Equals(invitation.InviterId)).Avatar
                };
            }
            yield break;
        };

        return new CommitResults<IdentityInvitationResponse>
        {
            ResultType = ResultType.Ok,
            Value = InvitationMapper()
        };

    }



}
