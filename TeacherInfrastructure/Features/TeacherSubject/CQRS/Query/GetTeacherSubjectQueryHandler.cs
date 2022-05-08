using TeacherDomain.Features.TeacherSubject.CQRS.Query;
using TeacherDomain.Features.TeacherSubject.DTO.Query;
using TeacherInfrastructure.HttpClients;
using DomianEntities = TeacherEntities.Entities.TeacherSubjects;

namespace TeacherInfrastructure.Features.TeacherSubject.CQRS.Query;
public class GetTeacherSubjectQueryHandler : IRequestHandler<GetTeacherSubjectQuery, CommitResults<TeacherSubjectResponse>>
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
    public async Task<CommitResults<TeacherSubjectResponse>?> Handle(GetTeacherSubjectQuery request, CancellationToken cancellationToken)
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
            return new CommitResults<TeacherSubjectResponse>
            {
                ResultType = ResultType.Empty,
                Value = Array.Empty<TeacherSubjectResponse>()
            };
        }
    }
}
