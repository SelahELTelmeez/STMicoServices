using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherEntities.Entities.TeacherClasses;
using DomainTeacherEntities = TeacherEntities.Entities.TeacherSubjects;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, ICommitResult<int>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _teacherId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public CreateClassCommandHandler(TeacherDbContext dbContext,
                                     IWebHostEnvironment configuration,
                                     IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _teacherId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResult<int>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {

        DomainTeacherEntities.TeacherSubject? teacherSubject = await _dbContext.Set<DomainTeacherEntities.TeacherSubject>().SingleOrDefaultAsync(a => a.TeacherId.Equals(_teacherId) && a.SubjectId.Equals(request.CreateClassRequest.SubjectId), cancellationToken);

        if (teacherSubject == null)
        {
            return ResultType.Invalid.GetValueCommitResult<int>(default, "X0002", _resourceJsonManager["X0002"]);
        }
        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>().SingleOrDefaultAsync(a => a.Name.Equals(request.CreateClassRequest.Name) && a.TeacherId.Equals(_teacherId), cancellationToken);

        if (teacherClass != null)
        {
            if (teacherSubject.IsDeleted == true)
            {
                return ResultType.Duplicated.GetValueCommitResult<int>(default, "X0004", _resourceJsonManager["X0004"]);
            }
            else
            {
                return ResultType.Duplicated.GetValueCommitResult<int>(default, "X0003", _resourceJsonManager["X0003"]);
            }
        }

        teacherClass = new TeacherClass
        {
            Description = request.CreateClassRequest.Description,
            Name = request.CreateClassRequest.Name,
            SubjectId = request.CreateClassRequest.SubjectId,
            TeacherId = _teacherId.GetValueOrDefault(),
            IsActive = true
        };

        _dbContext.Set<TeacherClass>().Add(teacherClass);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = teacherClass.Id
        };
    }
}
