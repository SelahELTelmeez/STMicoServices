using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherDomain.Services;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class UnrollStudentClassCommandHandler : IRequestHandler<UnrollStudentClassCommand, CommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly IMediator _mediator;
    private readonly IdentityClient _identityClient;
    private readonly INotificationService _notificationService;
    private readonly IHttpClientFactory _httpClientFactory;
    public UnrollStudentClassCommandHandler(IHttpContextAccessor httpContextAccessor, TeacherDbContext dbContext, IMediator mediator, IdentityClient identityClient, INotificationService notificationService, IHttpClientFactory httpClientFactory)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _mediator = mediator;
        _identityClient = identityClient;
        _notificationService = notificationService;
        _httpClientFactory = httpClientFactory;
    }
    public async Task<CommitResult> Handle(UnrollStudentClassCommand request, CancellationToken cancellationToken)
    {
        ClassEnrollee? classEnrollee = await _dbContext.Set<ClassEnrollee>()
                                      .Where(a => a.StudentId.Equals(_userId) && a.ClassId.Equals(request.ClassId))
                                      .Include(a => a.TeacherClassFK)
                                      .SingleOrDefaultAsync(cancellationToken);

        if (classEnrollee == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound
            };
        }
        else
        {
            classEnrollee.IsActive = false;
            _dbContext.Set<ClassEnrollee>().Update(classEnrollee);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        // ========================== Create a notification =======================================

        //DomainNotificationEntities.NotificationType? notificationType = await _dbContext.Set<DomainNotificationEntities.NotificationType>().SingleOrDefaultAsync(a => a.Id.Equals(3));
        //CommitResult<LimitedProfileResponse>? StudentLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(_userId.GetValueOrDefault(), cancellationToken);
        //if (!StudentLimitedProfile.IsSuccess)
        //{
        //    return StudentLimitedProfile.Adapt<CommitResult>();
        //}
        //await _mediator.Send(new CreateNotificationCommand(new NotificationRequest
        //{
        //    NotifierId = _userId.GetValueOrDefault(),
        //    ActorId = studentEnrollClass.TeacherClassFK.TeacherId,
        //    NotificationTypeId = 3,
        //    Argument = studentEnrollClass.ClassId.ToString(),
        //    Title = notificationType.Name,
        //    Message = StudentLimitedProfile.Value.FullName + " قد ألغى اشتراكه بفصل " + studentEnrollClass.TeacherClassFK.Name + " الخاص بالمعلم/المعلمة " + studentEnrollClass.TeacherClassFK.Name,
        //    IsSeen = false
        //}), cancellationToken);


        //CommitResult<LimitedProfileResponse>? TeacherLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(studentEnrollClass.TeacherClassFK.TeacherId, cancellationToken);
        //if (!TeacherLimitedProfile.IsSuccess)
        //{
        //    return TeacherLimitedProfile.Adapt<CommitResult>();
        //}
        //await _notificationService.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"), new NotificationModel
        //{
        //    Token = TeacherLimitedProfile.Value.NotificationToken,
        //    Type = 3,
        //    Title = notificationType.Name,
        //    Body = StudentLimitedProfile.Value.FullName + " قد ألغى اشتراكه بفصل " + studentEnrollClass.TeacherClassFK.Name
        //}, cancellationToken);


        // TODO: Notify parent 

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}