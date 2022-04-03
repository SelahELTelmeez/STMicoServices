using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using TransactionDomain.Features.IdentityInvitation.CQRS.Query;
using TransactionDomain.Features.IdentityInvitation.DTO;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.Invitation;

namespace TransactionInfrastructure.Features.IdentityInvitation.CQRS;

public class GetIdentityInvitationsQueryHandler : IRequestHandler<GetIdentityInvitationsQuery, CommitResult<IEnumerable<IdentityInvitationResponse>>>
{
    private readonly StudentTrackerDbContext _dbContext;
    private readonly HttpClient _IdentityClient;

    public GetIdentityInvitationsQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, StudentTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _IdentityClient = factory.CreateClient("IdentityClient");
        //_IdentityClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        //_IdentityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResult<IEnumerable<IdentityInvitationResponse>>> Handle(GetIdentityInvitationsQuery request, CancellationToken cancellationToken)
    {
        List<DomainEntities.Invitation> invitations = await _dbContext.Set<DomainEntities.Invitation>()
                                                                      .Where(a => a.InvitedId.Equals(request.IdentityId))
                                                                      .Include(a=> a.InvitationTypeFK)
                                                                      .OrderByDescending(a => a.CreatedOn).ToListAsync(cancellationToken);


        // Get List Of Identity Users
        // We Will remove Invitation Request
        HttpResponseMessage responseMessage = await _IdentityClient.PostAsJsonAsync("/identity/GetIdentityUserInvitations", invitations.Select(a => a.InviterId).ToList(), cancellationToken);

        List<IdentityUserInvitationResponse>? identityUserInvitationResponses = await responseMessage.Content.ReadFromJsonAsync<List<IdentityUserInvitationResponse>>();


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
                    Description = $"{invitation.InvitationTypeFK.Name} {identityUserInvitationResponses.SingleOrDefault(a => a.Id.Equals(invitation.InviterId)).FullName} {invitation.InvitationTypeFK.Description}",
                    AvatarUrl = identityUserInvitationResponses.SingleOrDefault(a=>a.Id.Equals(invitation.InviterId)).Avatar
                };
            }
            yield break;
        };

        return new CommitResult<IEnumerable<IdentityInvitationResponse>>
        {
            ResultType = ResultType.Ok,
            Value = InvitationMapper()
        };

    }

  
  
}
