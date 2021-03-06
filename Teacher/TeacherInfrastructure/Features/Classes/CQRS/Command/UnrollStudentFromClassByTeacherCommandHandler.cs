using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class UnrollStudentFromClassByTeacherCommandHandler : IRequestHandler<UnrollStudentFromClassByTeacherCommand, ICommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly string? _teacherId;
    private readonly NotifierClient _notifierClient;

    public UnrollStudentFromClassByTeacherCommandHandler(TeacherDbContext dbContext,
                                                         IWebHostEnvironment configuration,
                                                         IHttpContextAccessor httpContextAccessor,
                                                         NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _teacherId = httpContextAccessor.GetIdentityUserId();
        _notifierClient = notifierClient;
    }
    public async Task<ICommitResult> Handle(UnrollStudentFromClassByTeacherCommand request, CancellationToken cancellationToken)
    {
        ClassEnrollee? classEnrollee = await _dbContext.Set<ClassEnrollee>()
                                                       .Where(a => a.ClassId.Equals(request.RemoveStudentFromClassRequest.ClassId) && a.StudentId.Equals(request.RemoveStudentFromClassRequest.StudentId))
                                                       .Include(a => a.TeacherClassFK)
                                                       .FirstOrDefaultAsync(cancellationToken);

        if (classEnrollee == null)
        {
            return ResultType.NotFound.GetCommitResult("XTEC0011", _resourceJsonManager["XTEC0011"]);
        }

        classEnrollee.IsActive = false;
        _dbContext.Set<ClassEnrollee>().Update(classEnrollee);
        await _dbContext.SaveChangesAsync(cancellationToken);

        await _notifierClient.SendNotificationAsync(new NotificationRequest
        {
            NotifiedId = request.RemoveStudentFromClassRequest.StudentId,
            NotifierId = _teacherId,
            Argument = request.RemoveStudentFromClassRequest.ClassId.ToString(),
            NotificationTypeId = 6,
            AppenedMessage = classEnrollee.TeacherClassFK.Name
        }, cancellationToken);


        return ResultType.Ok.GetCommitResult();
    }
}