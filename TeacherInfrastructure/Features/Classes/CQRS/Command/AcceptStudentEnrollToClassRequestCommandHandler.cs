using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;
using DomainEntities = TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class AcceptStudentEnrollToClassRequestCommandHandler : IRequestHandler<AcceptStudentEnrollToClassRequestCommand, ICommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly NotifierClient _notifierClient;

    public AcceptStudentEnrollToClassRequestCommandHandler(TeacherDbContext dbContext,
                                                        IWebHostEnvironment configuration,
                                                        IHttpContextAccessor httpContextAccessor, NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _notifierClient = notifierClient;
    }
    public async Task<ICommitResult?> Handle(AcceptStudentEnrollToClassRequestCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>()
                                                                    .Include(a => a.ClassEnrollees)
                                                                    .SingleOrDefaultAsync(a => a.Id.Equals(request.AcceptStudentEnrollToClassRequest.ClassId), cancellationToken);

        if (teacherClass == null)
        {
            return ResultType.NotFound.GetCommitResult("XTEC0001", _resourceJsonManager["XTEC0001"]);
        }

        if (teacherClass.ClassEnrollees.Any(a => a.StudentId.Equals(request.AcceptStudentEnrollToClassRequest.StudentId)))
        {
            ClassEnrollee? classEnrollee = teacherClass.ClassEnrollees.Single(a => a.StudentId.Equals(request.AcceptStudentEnrollToClassRequest.StudentId));
            classEnrollee.IsActive = true;
            _dbContext.Set<ClassEnrollee>().Update(classEnrollee);
        }
        else
        {
            teacherClass.ClassEnrollees.Add(new ClassEnrollee
            {
                ClassId = request.AcceptStudentEnrollToClassRequest.ClassId,
                StudentId = request.AcceptStudentEnrollToClassRequest.StudentId,
                IsActive = true
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return await _notifierClient.SetAsInActiveInvitationAsync(request.AcceptStudentEnrollToClassRequest.InvitationId, 1, cancellationToken);
    }
}
