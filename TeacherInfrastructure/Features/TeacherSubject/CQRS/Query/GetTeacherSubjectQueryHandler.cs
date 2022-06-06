using SharedModule.DTO;
using TeacherDomain.Features.TeacherSubject.CQRS.Query;
using TeacherInfrastructure.HttpClients;
using DomianEntities = TeacherEntities.Entities.TeacherSubjects;

namespace TeacherInfrastructure.Features.TeacherSubject.CQRS.Query;
public class GetTeacherSubjectQueryHandler : IRequestHandler<GetTeacherSubjectQuery, ICommitResults<TeacherSubjectResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _teacherId;
    private readonly CurriculumClient _CurriculumClient;

    public GetTeacherSubjectQueryHandler(TeacherDbContext dbContext, IHttpContextAccessor httpContextAccessor, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _teacherId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = curriculumClient;
    }
    public async Task<ICommitResults<TeacherSubjectResponse>?> Handle(GetTeacherSubjectQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<DomianEntities.TeacherSubject>? teacherSubjects = await _dbContext.Set<DomianEntities.TeacherSubject>()
                                                                                      .Where(a => a.TeacherId.Equals(_teacherId))
                                                                                      .ToListAsync(cancellationToken);
        if (teacherSubjects.Any())
        {
            return await _CurriculumClient.GetTeacherSubjectsDetailsAsync(teacherSubjects.Select(a => a.SubjectId), cancellationToken);
        }
        else
        {
            return ResultType.Empty.GetValueCommitResults(Array.Empty<TeacherSubjectResponse>());
        }
    }
}
