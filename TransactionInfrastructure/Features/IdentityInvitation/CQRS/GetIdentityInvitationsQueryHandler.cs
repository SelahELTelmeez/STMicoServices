using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
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
        _IdentityClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _IdentityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResult<IEnumerable<IdentityInvitationResponse>>> Handle(GetIdentityInvitationsQuery request, CancellationToken cancellationToken)
    {
        List<DomainEntities.Invitation> invitations = await _dbContext.Set<DomainEntities.Invitation>()
                                                                      .Where(a => a.InvitedId.Equals(request.IdentityId)).OrderByDescending(a => a.CreatedOn).ToListAsync(cancellationToken);

        //async IEnumerable<IdentityInvitationResponse> InvitationMapper()
        //{
        //    foreach (DomainEntities.Invitation invitation in invitations)
        //    {
        //        IdentityDetailsInvitationResponse? identityDetailsResponse = await _IdentityClient.GetFromJsonAsync<IdentityDetailsInvitationResponse>("/Identity/GetIdentityDetails", cancellationToken:cancellationToken);

        //        yield return new IdentityInvitationResponse
        //        {
        //            CreatedOn = invitation.CreatedOn.GetValueOrDefault(),
        //            InvitationId = invitation.Id,
        //            IsNew = invitation.IsNew,
        //            IsSeen = invitation.IsSeen,
        //            Status = (int)invitation.Status,
        //            Description = $"",
        //            AvatarUrl = ""

        //        };
        //    }
        //    yield break;
        //};

        return new CommitResult<IEnumerable<IdentityInvitationResponse>>
        {
            ResultType = ResultType.Ok,

        };

    }
}
