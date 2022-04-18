using Mapster;
using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherDomain.Features.Shared.DTO;
using TeacherDomain.Services;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class RemoveStudentFromClassCommandHandler : IRequestHandler<RemoveStudentFromClassCommand, CommitResult>
{
    private IMediator _mediator;
    private readonly INotificationService _notification;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IdentityClient _identityClient;
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly Guid? _userId;


    public RemoveStudentFromClassCommandHandler(
            IMediator mediator,
            TeacherDbContext dbContext,
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
    public async Task<CommitResult> Handle(RemoveStudentFromClassCommand request, CancellationToken cancellationToken)
    {
        CommitResults<LimitedProfileResponse>? profileResponses = await _identityClient.GetIdentityLimitedProfilesAsync(new Guid[] { request.RemoveStudentFromClassRequest.StudentId, _userId.GetValueOrDefault() }, cancellationToken);

        if (!profileResponses.IsSuccess)
        {
            return profileResponses.Adapt<CommitResult>();
        }

        ClassEnrollee? classEnrollee = await _dbContext.Set<ClassEnrollee>()
            .Where(a => a.ClassId.Equals(request.RemoveStudentFromClassRequest.ClassId) && a.StudentId.Equals(request.RemoveStudentFromClassRequest.StudentId))
            .Include(a => a.TeacherClassFK)
            .SingleOrDefaultAsync(cancellationToken);

        if (classEnrollee == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound
            };
        }

        classEnrollee.IsActive = false;
        _dbContext.Set<ClassEnrollee>().Update(classEnrollee);
        await _dbContext.SaveChangesAsync(cancellationToken);


        //NotificationType? notificationType = await _dbContext.Set<NotificationType>().SingleOrDefaultAsync(a => a.Id.Equals(6));

        //LimitedProfileResponse studentProfile = profileResponses.Value.Single(a => a.UserId.Equals(request.RemoveStudentFromClassRequest.StudentId));
        //LimitedProfileResponse TeacherProfile = profileResponses.Value.Single(a => a.UserId.Equals(_userId));

        //await _mediator.Send(new CreateNotificationCommand(new NotificationRequest
        //{
        //    NotifierId = _userId.GetValueOrDefault(),
        //    ActorId = request.RemoveStudentFromClassRequest.StudentId,
        //    NotificationTypeId = 6,
        //    Argument = request.RemoveStudentFromClassRequest.ClassId.ToString(),
        //    Title = notificationType.Name,
        //    Message = TeacherProfile.FullName + " قد ألغى اشتراكك بفصل " + studentEnroll.TeacherClassFK.Name,
        //    IsSeen = false
        //}), cancellationToken);


        //await _notification.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
        //{
        //    Token = studentProfile.NotificationToken,
        //    Type = 4,
        //    Title = notificationType.Name,
        //    Body = TeacherProfile.FullName + " قد ألغى اشتراكك بفصل " + studentEnroll.TeacherClassFK.Name

        //}, cancellationToken);


        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}