using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class ToggleActivateClassCommandHandler : IRequestHandler<ToggleActivateClassCommand, ICommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public ToggleActivateClassCommandHandler(TeacherDbContext dbContext,
                                             IWebHostEnvironment configuration,
                                             IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }


    public async Task<ICommitResult> Handle(ToggleActivateClassCommand request, CancellationToken cancellationToken)
    {
        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>().SingleOrDefaultAsync(a => a.Id.Equals(request.ClassId) && a.TeacherId.Equals(_userId), cancellationToken);
        if (teacherClass == null)
        {
            return ResultType.NotFound.GetCommitResult("X0005", _resourceJsonManager["X0005"]);
        }
        teacherClass.IsActive = !teacherClass.IsActive;
        _dbContext.Set<TeacherClass>().Update(teacherClass);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return ResultType.Ok.GetCommitResult();
    }
}
