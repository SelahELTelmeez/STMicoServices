using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.Invitations.CQRS.DTO.Query;
using NotifierDomain.Features.Invitations.CQRS.Query;
using NotifierEntities.Entities.Invitations;
using NotifierInfrastructure.HttpClients;
using NotifierInfrastructure.Utilities;
using ResultHandler;
using NotifierEntities.Entities;

namespace NotifierInfrastructure.Features.Invitations.CQRS.Query;
public class GetIdentityInvitationsQueryHandler : IRequestHandler<GetIdentityInvitationsQuery, CommitResults<IdentityInvitationResponse>>
{
    private readonly NotifierDbContext _dbContext;
    private readonly IdentityClient _IdentityClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetIdentityInvitationsQueryHandler(IdentityClient identityClient, IHttpContextAccessor httpContextAccessor, NotifierDbContext dbContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _IdentityClient = identityClient;
    }
    public async Task<CommitResults<IdentityInvitationResponse>> Handle(GetIdentityInvitationsQuery request, CancellationToken cancellationToken)
    {
        List<Invitation> invitations = await _dbContext.Set<Invitation>()
                                                                      .Where(a => a.InvitedId.Equals(_httpContextAccessor.GetIdentityUserId()))
                                                                      .Include(a => a.InvitationTypeFK)
                                                                      .OrderByDescending(a => a.CreatedOn).ToListAsync(cancellationToken);
        // Get List Of Identity Users
        // We Will remove Invitation Request
        CommitResults<IdentityUserInvitationResponse>? identityUserInvitationResponses = await _IdentityClient.GetIdentityUserInvitationsAsync(invitations.Select(a => a.InviterId), cancellationToken);


        IEnumerable<IdentityInvitationResponse> Mapper()
        {
            foreach (Invitation invitation in invitations)
            {
                yield return new IdentityInvitationResponse
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
        return new CommitResultsProvider<IdentityInvitationResponse>().SuccessResult(Mapper());
    }
}
