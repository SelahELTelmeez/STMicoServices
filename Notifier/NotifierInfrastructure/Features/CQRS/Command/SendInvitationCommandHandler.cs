using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using NotifierDomain.Features.CQRS.Command;
using NotifierDomain.Models;
using NotifierDomain.Services;
using NotifierEntities.Entities;
using NotifierEntities.Entities.Invitations;
using NotifierInfrastructure.HttpClients;
using SharedModule.DTO;
using SharedModule.Extensions;

namespace NotifierInfrastructure.Features.CQRS.Command;
public class SendInvitationCommandHandler : IRequestHandler<SendInvitationCommand, ICommitResult>
{
    private readonly NotifierDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IdentityClient _identityClient;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;
    public SendInvitationCommandHandler(NotifierDbContext dbContext,
                                          IWebHostEnvironment configuration,
                                          IHttpContextAccessor httpContextAccessor,
                                          IdentityClient identityClient,
                                          INotificationService notification,
                                          IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _identityClient = identityClient;
        _notification = notification;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<ICommitResult> Handle(SendInvitationCommand request, CancellationToken cancellationToken)
    {
        Invitation? invitation = await _dbContext.Set<Invitation>()
                                                 .Where(a => a.InviterId.Equals(request.InvitationRequest.InviterId)
                                                         && a.InvitedId.Equals(request.InvitationRequest.InvitedId)
                                                         && a.Argument.Equals(request.InvitationRequest.Argument)
                                                         && a.IsActive == true)
                                                 .FirstOrDefaultAsync(cancellationToken);

        if (invitation != null)
        {
            return ResultType.Duplicated.GetCommitResult("X0001", _resourceJsonManager["X0001"]);
        }

        InvitationType? invitationType = await _dbContext.Set<InvitationType>().FirstOrDefaultAsync(a => a.Id.Equals(request.InvitationRequest.InvitationTypeId), cancellationToken);
        if (invitationType == null)
        {
            return ResultType.NotFound.GetCommitResult("X0002", _resourceJsonManager["X0002"]);
        }

        ICommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetLimitedProfilesAsync(new string[] { request.InvitationRequest.InvitedId, request.InvitationRequest.InviterId }, cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return ResultType.Invalid.GetCommitResult(limitedProfiles.ErrorCode, limitedProfiles.ErrorMessage);
        }

        LimitedProfileResponse? InvitedProfile = limitedProfiles.Value.FirstOrDefault(a => a.UserId.Equals(request.InvitationRequest.InvitedId));

        LimitedProfileResponse? InviterProfile = limitedProfiles.Value.FirstOrDefault(a => a.UserId.Equals(request.InvitationRequest.InviterId));

        string notificationBody = $"{InviterProfile?.FullName} {invitationType.Description} {request.InvitationRequest.AppenedMessage}";

        _dbContext.Set<Invitation>().Add(new Invitation
        {
            Argument = request.InvitationRequest.Argument ?? string.Empty,
            InvitedId = request.InvitationRequest.InvitedId,
            InviterId = request.InvitationRequest.InviterId,
            IsActive = true,
            InvitationTypeId = request.InvitationRequest.InvitationTypeId,
            Status = NotifierEntities.Entities.Shared.InvitationStatus.Pending,
            Message = notificationBody,
            Title = invitationType.Name,
            IsSeen = false
        });
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notification.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
        {
            Token = InvitedProfile?.NotificationToken ?? string.Empty,
            Type = request.InvitationRequest.InvitationTypeId,
            Title = invitationType.Name,
            Body = notificationBody
        }, cancellationToken);

        return ResultType.Ok.GetCommitResult();

    }
}