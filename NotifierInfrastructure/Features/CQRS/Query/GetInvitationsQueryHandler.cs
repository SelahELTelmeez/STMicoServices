using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierDomain.Features.CQRS.DTO.Query;
using NotifierDomain.Features.CQRS.Query;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using NotifierInfrastructure.HttpClients;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Query;
public class GetInvitationsQueryHandler : IRequestHandler<GetInvitationsQuery, ICommitResults<InvitationResponse>>
{
    private readonly NotifierDbContext _dbContext;
    private readonly IdentityClient _IdentityClient;
    private readonly Guid? _userId;
    private readonly IMediator _mediator;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetInvitationsQueryHandler(IdentityClient identityClient,
                                      IWebHostEnvironment configuration,
                                      IHttpContextAccessor httpContextAccessor,
                                      NotifierDbContext dbContext,
                                      IMediator mediator)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _IdentityClient = identityClient;
        _mediator = mediator;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

    }
    public async Task<ICommitResults<InvitationResponse>> Handle(GetInvitationsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Invitation> invitations = await _dbContext.Set<Invitation>()
                                                              .Where(a => a.InvitedId.Equals(_userId))
                                                              .Include(a => a.InvitationTypeFK)
                                                              .OrderByDescending(a => a.CreatedOn)
                                                              .ToListAsync(cancellationToken);

        if (!invitations.Any())
        {
            return ResultType.Empty.GetValueCommitResults(Array.Empty<InvitationResponse>(), "X0007", _resourceJsonManager["X0007"]);
        }

        ICommitResults<LimitedProfileResponse>? limitedProfiles = await _IdentityClient.GetLimitedProfilesAsync(invitations.Select(a => a.InviterId), cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return ResultType.Invalid.GetValueCommitResults<InvitationResponse>(default, limitedProfiles.ErrorCode, limitedProfiles.ErrorMessage);

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

        _ = _mediator.Send(new SetAsSeenInvitationCommand(), cancellationToken);

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}
