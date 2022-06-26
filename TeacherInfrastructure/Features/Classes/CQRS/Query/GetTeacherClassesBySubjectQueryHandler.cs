
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.TeacherClass.DTO.Query;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetTeacherClassesBySubjectQueryHandler : IRequestHandler<GetTeacherClassesBySubjectQuery, ICommitResults<TeacherClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetTeacherClassesBySubjectQueryHandler(TeacherDbContext dbContext,
                                                  IWebHostEnvironment configuration,
                                                  IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<ICommitResults<TeacherClassResponse>> Handle(GetTeacherClassesBySubjectQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                   .Where(a => a.SubjectId.Equals(request.SubjectId) && a.TeacherId.Equals(_userId) && a.IsActive)
                                                                   .Include(a => a.ClassEnrollees)
                                                                   .ToListAsync(cancellationToken);

        if (!teacherClasses.Any())
        {
            return ResultType.Ok.GetValueCommitResults(Array.Empty<TeacherClassResponse>());
        }

        IEnumerable<TeacherClassResponse> Mapper()
        {
            foreach (TeacherClass teacherClass in teacherClasses)
            {
                yield return new TeacherClassResponse
                {
                    Description = teacherClass.Description,
                    SubjectId = teacherClass.SubjectId,
                    Name = teacherClass.Name,
                    IsActive = teacherClass.IsActive,
                    Id = teacherClass.Id,
                    EnrollersCount = teacherClass.ClassEnrollees.Where(a => a.IsActive).Count(),
                };
            }
            yield break;
        }

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}
