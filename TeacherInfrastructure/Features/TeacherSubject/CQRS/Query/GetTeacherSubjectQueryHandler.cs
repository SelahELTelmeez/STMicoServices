using SharedModule.DTO;
using TeacherDomain.Features.TeacherSubject.CQRS.Query;
using TeacherInfrastructure.HttpClients;
using DomianEntities = TeacherEntities.Entities.TeacherSubjects;

namespace TeacherInfrastructure.Features.TeacherSubject.CQRS.Query;
public class GetTeacherSubjectQueryHandler : IRequestHandler<GetTeacherSubjectQuery, ICommitResults<TeacherSubjectResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly string? _teacherId;
    private readonly CurriculumClient _CurriculumClient;
    private readonly JsonLocalizerManager _resourceJsonManager;


    public GetTeacherSubjectQueryHandler(TeacherDbContext dbContext,
                                         IHttpContextAccessor httpContextAccessor,
                                         IWebHostEnvironment configuration,
                                         CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _teacherId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = curriculumClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

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
            return ResultType.Ok.GetValueCommitResults(Array.Empty<TeacherSubjectResponse>());
        }
    }
}
