using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
/// <summary>
/// Represents Student Get himself out of a class.
/// </summary>
public class UnrollStudentClassCommandHandler : IRequestHandler<UnrollStudentClassCommand, CommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly NotifierClient _notifierClient;
    public UnrollStudentClassCommandHandler(IHttpContextAccessor httpContextAccessor, TeacherDbContext dbContext, NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _notifierClient = notifierClient;
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

        await _notifierClient.SendNotificationAsync(new NotificationRequest
        {
            NotifierId = _userId.GetValueOrDefault(),
            NotifiedId = classEnrollee.TeacherClassFK.TeacherId,
            NotificationTypeId = 3,
            Argument = classEnrollee.ClassId.ToString(),
            AppenedMessage = $"{classEnrollee.TeacherClassFK.Name}  الخاص بالمعلم / المعلمة {classEnrollee.TeacherClassFK.Name}",
        }, cancellationToken);


        // TODO: Notify parent 

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}