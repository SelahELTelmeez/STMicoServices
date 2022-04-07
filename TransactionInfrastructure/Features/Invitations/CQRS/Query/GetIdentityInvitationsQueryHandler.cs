using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransactionDomain.Features.Invitations.CQRS.DTO.Query;
using TransactionDomain.Features.Invitations.CQRS.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.Invitation;

namespace TransactionInfrastructure.Features.Invitations.CQRS.Query;
public class GetIdentityInvitationsQueryHandler : IRequestHandler<GetIdentityInvitationsQuery, CommitResults<IdentityInvitationResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly HttpClient _IdentityClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetIdentityInvitationsQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _IdentityClient = factory.CreateClient("IdentityClient");
        _IdentityClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _IdentityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResults<IdentityInvitationResponse>> Handle(GetIdentityInvitationsQuery request, CancellationToken cancellationToken)
    {
        List<DomainEntities.Invitation> invitations = await _dbContext.Set<DomainEntities.Invitation>()
                                                                      .Where(a => a.InvitedId.Equals(_httpContextAccessor.GetIdentityUserId()))
                                                                      .Include(a => a.InvitationTypeFK)
                                                                      .OrderByDescending(a => a.CreatedOn).ToListAsync(cancellationToken);
        // Get List Of Identity Users
        // We Will remove Invitation Request
        HttpResponseMessage responseMessage = await _IdentityClient.PostAsJsonAsync("/Identity/GetIdentityUserInvitations", invitations.Select(a => a.InviterId), cancellationToken);

        List<IdentityUserInvitationResponse>? identityUserInvitationResponses = await responseMessage.Content.ReadFromJsonAsync<List<IdentityUserInvitationResponse>>(cancellationToken: cancellationToken);


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
                    AvatarUrl = identityUserInvitationResponses.SingleOrDefault(a => a.Id.Equals(invitation.InviterId)).Avatar
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
