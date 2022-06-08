using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Reports.CQRS.Query;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Reports.CQRS.Query;
public class GetSubjectDetailedProgressQueryHandler : IRequestHandler<GetSubjectDetailedProgressQuery, ICommitResult<DetailedProgressResponse>>
{
    private readonly StudentDbContext _dbContext;
    private readonly CurriculumClient _curriculumClient;
    private readonly Guid? _studentId;

    public GetSubjectDetailedProgressQueryHandler(StudentDbContext dbContext, CurriculumClient curriculumClient, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _curriculumClient = curriculumClient;
        _studentId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<ICommitResult<DetailedProgressResponse>> Handle(GetSubjectDetailedProgressQuery request, CancellationToken cancellationToken)
    {
        ICommitResult<DetailedProgressResponse>? detailedProgress = await _curriculumClient.GetSubjectDetailedProgressAsync(request.SubjectId, cancellationToken);
        if (!detailedProgress.IsSuccess)
        {
            detailedProgress.ResultType.GetValueCommitResult((DetailedProgressResponse)null, detailedProgress.ErrorCode, detailedProgress.ErrorMessage);
        }

        IEnumerable<ActivityTracker> studentActivities = await _dbContext.Set<ActivityTracker>()
                                                                         .Where(a => a.StudentId == (request.SudentId ?? _studentId.GetValueOrDefault()))
                                                                         .Where(a => a.SubjectId == request.SubjectId)
                                                                         .Where(a => a.IsActive == true)
                                                                         .ToListAsync(cancellationToken);

        foreach (DetailedLessonProgress lesson in detailedProgress.Value.UnitProgresses.SelectMany(a => a.LessonProgresses))
        {
            var response = studentActivities?.Where(a => a.LessonId == lesson.LessonId);
            if (response.Any())
            {
                lesson.TotalLessonStudentScore = response?.Max(a => a.StudentPoints) ?? 0;
            }
        }

        return detailedProgress;
    }
}
