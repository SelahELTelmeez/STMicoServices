using TeacherDomain.Features.TeacherSubject.CQRS.Command;
using DomianEntities = TeacherEntities.Entities.TeacherSubjects;

namespace TeacherInfrastructure.Features.TeacherSubject.CQRS.Command;
public class AddTeacherSubjectCommandHandler : IRequestHandler<AddTeacherSubjectCommand, ICommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly string? _userId;

    public AddTeacherSubjectCommandHandler(TeacherDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<ICommitResult> Handle(AddTeacherSubjectCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<DomianEntities.TeacherSubject>? teacherSubjects = await _dbContext.Set<DomianEntities.TeacherSubject>()
                                                                                      .Where(a => a.TeacherId.Equals(_userId))
                                                                                      .ToListAsync(cancellationToken);

        IEnumerable<DomianEntities.TeacherSubject>? removedSubjects = teacherSubjects?.Where(a => !request.SubjectIds.Contains(a.SubjectId))?.ToList();
        IEnumerable<DomianEntities.TeacherSubject>? existedSubjects = teacherSubjects?.Where(a => request.SubjectIds.Contains(a.SubjectId))?.ToList();

        if (removedSubjects?.Any() ?? false)
        {
            _dbContext.Set<DomianEntities.TeacherSubject>().RemoveRange(removedSubjects);
        }

        foreach (string SubjectId in request.SubjectIds.Where(a => !removedSubjects.Concat(existedSubjects).Select(b => b.SubjectId).Contains(a)))
        {
            _dbContext.Set<DomianEntities.TeacherSubject>().Add(new DomianEntities.TeacherSubject
            {
                SubjectId = SubjectId,
                TeacherId = _userId
            });
        }


        await _dbContext.SaveChangesAsync(cancellationToken);
        return ResultType.Ok.GetCommitResult();
    }
}
