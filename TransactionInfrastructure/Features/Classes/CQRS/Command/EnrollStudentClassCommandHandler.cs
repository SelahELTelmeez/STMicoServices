using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.CQRS.Command;

public class EnrollStudentClassCommandHandler : IRequestHandler<EnrollStudentClassCommand, CommitResult>
{
    private IMediator _mediator;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdentityClient _identityClient;
    private readonly TrackerDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly Guid? _userId;


    public EnrollStudentClassCommandHandler(
            IMediator mediator,
            TrackerDbContext dbContext,
            INotificationService notification,
            IHttpClientFactory httpClientFactory,
            IdentityClient identityClient,
            IWebHostEnvironment configuration,
            IHttpContextAccessor httpContextAccessor)
    {
        _mediator = mediator;
        _notification = notification;
        _httpClientFactory = httpClientFactory;
        _identityClient = identityClient;
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _userId = httpContextAccessor.GetIdentityUserId();

    }
    public async Task<CommitResult> Handle(EnrollStudentClassCommand request, CancellationToken cancellationToken)
    {

        CommitResult<LimitedProfileResponse>? limitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(_userId.GetValueOrDefault(), cancellationToken);
        if (!limitedProfile.IsSuccess)
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
        DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>().SingleOrDefaultAsync(a => a.Id.Equals(request.ClassId), cancellationToken);
        if (teacherClass == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"]
            };
        }
        CommitResult createInvitationResult = await _mediator.Send(new CreateInvitationCommand(new InvitationRequest
        {
            InviterId = _userId.GetValueOrDefault(),
            InvitedId = teacherClass.TeacherId,
            Argument = request.ClassId.ToString(),
            InvitationTypeId = 4,
            IsActive = true,
        }), cancellationToken);

        if (!createInvitationResult.IsSuccess)
        {
            return createInvitationResult.Adapt<CommitResult>();
        }

        CommitResult<LimitedProfileResponse>? TeacherLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(teacherClass.TeacherId, cancellationToken);

        if (!TeacherLimitedProfile.IsSuccess)
        {
            return TeacherLimitedProfile.Adapt<CommitResult>();
        }
        await _notification.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
        {
            Token = TeacherLimitedProfile.Value.NotificationToken,
            Type = 4,
            Title = invitationType.Name,
            Body = limitedProfile.Value.FullName + " " + invitationType.Description + " " + teacherClass.Name

        }, cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
