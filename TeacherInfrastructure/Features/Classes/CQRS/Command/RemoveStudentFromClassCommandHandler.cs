using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class RemoveStudentFromClassCommandHandler : IRequestHandler<RemoveStudentFromClassCommand, CommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly Guid? _userId;
    private readonly NotifierClient _notifierClient;

    public RemoveStudentFromClassCommandHandler(
            TeacherDbContext dbContext,
            IWebHostEnvironment configuration,
            IHttpContextAccessor httpContextAccessor,
            NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _userId = httpContextAccessor.GetIdentityUserId();
        _notifierClient = notifierClient;
    }
    public async Task<CommitResult> Handle(RemoveStudentFromClassCommand request, CancellationToken cancellationToken)
    {
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

        await _notifierClient.SendNotificationAsync(new NotificationRequest
        {
            NotifiedId = request.RemoveStudentFromClassRequest.StudentId,
            NotifierId = _userId.GetValueOrDefault(),
            Argument = request.RemoveStudentFromClassRequest.ClassId.ToString(),
            NotificationTypeId = 6,
            AppenedMessage = classEnrollee.TeacherClassFK.Name
        }, cancellationToken);


        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}