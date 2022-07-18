using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
/// <summary>
/// Represents Student Get himself out of a class.
/// </summary>
public class UnrollFromClassCommandHandler : IRequestHandler<UnrollFromClassCommand, ICommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly string? _studentId;
    private readonly NotifierClient _notifierClient;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UnrollFromClassCommandHandler(IHttpContextAccessor httpContextAccessor,
                                         IWebHostEnvironment configuration,
                                         TeacherDbContext dbContext,
                                         NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _studentId = httpContextAccessor.GetIdentityUserId();
        _notifierClient = notifierClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<ICommitResult> Handle(UnrollFromClassCommand request, CancellationToken cancellationToken)
    {
        ClassEnrollee? classEnrollee = await _dbContext.Set<ClassEnrollee>()
                                                       .Where(a => a.StudentId.Equals(_studentId) && a.ClassId.Equals(request.ClassId))
                                                       .Include(a => a.TeacherClassFK)
                                                       .FirstOrDefaultAsync(cancellationToken);

        if (classEnrollee == null)
        {
            return ResultType.NotFound.GetCommitResult("XTEC0005", _resourceJsonManager["XTEC0005"]);
        }
        else
        {
            classEnrollee.IsActive = false;
            _dbContext.Set<ClassEnrollee>().Update(classEnrollee);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        // ========================== Create a notification =======================================

        await _notifierClient.SendNotificationAsync(new NotificationRequest
        {
            NotifierId = _studentId,
            NotifiedId = classEnrollee.TeacherClassFK.TeacherId,
            NotificationTypeId = 3,
            Argument = classEnrollee.ClassId.ToString(),
            AppenedMessage = $"{classEnrollee.TeacherClassFK.Name}  الخاص بالمعلم / المعلمة {classEnrollee.TeacherClassFK.Name}",
        }, cancellationToken);


        // TODO: Notify parent 

        return ResultType.Ok.GetCommitResult();
    }
}