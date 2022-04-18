using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherEntites.Entities.TeacherClasses;
using DomainEntities = TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class AcceptStudentEnrollmentToClassCommandHandler : IRequestHandler<AcceptStudentEnrollmentToClassCommand, CommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public AcceptStudentEnrollmentToClassCommandHandler(TeacherDbContext dbContext,
                                                        IWebHostEnvironment configuration,
                                                        IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<CommitResult> Handle(AcceptStudentEnrollmentToClassCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>()
                                                                    .Include(a => a.ClassEnrollees)
                                                                    .SingleOrDefaultAsync(a => a.Id.Equals(request.AddStudentToClassRequest.ClassId), cancellationToken);

        if (teacherClass == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0001"]
            };
        }

        if (teacherClass.ClassEnrollees.Any(a => a.StudentId.Equals(request.AddStudentToClassRequest.StudentId)))
        {
            ClassEnrollee? classEnrollee = teacherClass.ClassEnrollees.Single(a => a.StudentId.Equals(request.AddStudentToClassRequest.StudentId));
            classEnrollee.IsActive = true;
            _dbContext.Set<ClassEnrollee>().Update(classEnrollee);
        }
        else
        {
            teacherClass.ClassEnrollees.Add(new ClassEnrollee
            {
                ClassId = request.AddStudentToClassRequest.ClassId,
                StudentId = request.AddStudentToClassRequest.StudentId,
                IsActive = true
            });
        }



        //Invitation? invitation = await _dbContext.Set<Invitation>().SingleOrDefaultAsync(a => a.Id.Equals(request.AddStudentToClassRequest.InvitationId), cancellationToken);

        //if (invitation == null)
        //{
        //    return new CommitResult
        //    {
        //        ResultType = ResultType.NotFound,
        //        ErrorCode = "X0000",
        //        ErrorMessage = _resourceJsonManager["X0001"]
        //    };
        //}
        //invitation.IsActive = false;
        //_dbContext.Set<Invitation>().Update(invitation);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
