using JsonLocalizer;
using Mapster;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Command;
using TransactionDomain.Features.Invitations.CQRS.Command;
using TransactionDomain.Features.Invitations.CQRS.DTO.Command;
using TransactionDomain.Features.Shared.DTO;
using TransactionDomain.Models;
using TransactionDomain.Services;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Invitation;
using TransactionInfrastructure.HttpClients;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.CQRS.Command;

public class InviteStudentToClassCommandHandler : IRequestHandler<InviteStudentToClassCommand, CommitResult>
{
    private IMediator _mediator;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdentityClient _identityClient;
    private readonly TrackerDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public InviteStudentToClassCommandHandler(
            IMediator mediator,
            TrackerDbContext dbContext,
            INotificationService notification,
            IHttpClientFactory httpClientFactory,
            IdentityClient identityClient,
            JsonLocalizerManager resourceJsonManager)
    {
        _mediator = mediator;
        _notification = notification;
        _httpClientFactory = httpClientFactory;
        _identityClient = identityClient;
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }
    public async Task<CommitResult> Handle(InviteStudentToClassCommand request, CancellationToken cancellationToken)
    {
        CommitResult createInvitationResult = await _mediator.Send(new CreateInvitationCommand(new InvitationRequest
        {
            InviterId = request.InviteStudentToClassRequest.TeacherId,
            InvitedId = request.InviteStudentToClassRequest.StudentId,
            Argument = request.InviteStudentToClassRequest.ClassId.ToString(),
            InvitationTypeId = 4,
            IsActive = true,
        }), cancellationToken);

        if (createInvitationResult.IsSuccess)
        {
            CommitResult<LimitedProfileResponse>? limitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(request.InviteStudentToClassRequest.StudentId, cancellationToken);
            if (!limitedProfile?.IsSuccess ?? true)
            {
                return limitedProfile.Adapt<CommitResult>();
            }
            InvitationType? invitationType = await _dbContext.Set<InvitationType>().SingleOrDefaultAsync(a => a.Id.Equals(4), cancellationToken);
            if (invitationType == null)
            {
                return new CommitResult
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "X0000",
                    ErrorMessage = _resourceJsonManager["X0000"]
                };
            }
            DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>().SingleOrDefaultAsync(a => a.Id.Equals(request.InviteStudentToClassRequest.ClassId), cancellationToken);
            if (teacherClass == null)
            {
                return new CommitResult
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "X0000",
                    ErrorMessage = _resourceJsonManager["X0000"]
                };
            }
            await _notification.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
            {
                Token = limitedProfile.Value.NotificationToken,
                Type = 4,
                Title = invitationType.Name,
                Body = limitedProfile.Value.FullName + " " + invitationType.Description + " " + teacherClass.Name

            }, cancellationToken);

            return new CommitResult
            {
                ResultType = ResultType.Ok
            };
        }
        else
        {
            return createInvitationResult.Adapt<CommitResult>();
        }

    }
}
