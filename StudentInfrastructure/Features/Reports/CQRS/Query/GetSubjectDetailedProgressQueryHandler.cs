using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Reports.CQRS.Query;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Reports.CQRS.Query;
public class GetSubjectDetailedProgressQueryHandler : IRequestHandler<GetSubjectDetailedProgressQuery, CommitResult<DetailedProgressResponse>>
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
    public async Task<CommitResult<DetailedProgressResponse>> Handle(GetSubjectDetailedProgressQuery request, CancellationToken cancellationToken)
    {
        CommitResult<DetailedProgressResponse>? detailedProgress = await _curriculumClient.GetSubjectDetailedProgressAsync(request.SubjectId, cancellationToken);
        if (!detailedProgress.IsSuccess)
        {
            return new CommitResult<DetailedProgressResponse>
            {
                ErrorCode = detailedProgress.ErrorCode,
                ErrorMessage = detailedProgress.ErrorMessage,
                ResultType = detailedProgress.ResultType
            };
        }

        IEnumerable<ActivityTracker> studentActivities = await _dbContext.Set<ActivityTracker>()
                                                                         .Where(a => a.StudentId == (request.SudentId ?? _studentId.GetValueOrDefault()))
                                                                         .Where(a => a.SubjectId == request.SubjectId)
                                                                         .ToListAsync(cancellationToken);


        detailedProgress.Value.TotalSubjectStudentScore = studentActivities.Where(a => a.SubjectId == request.SubjectId)
                                                                           .Sum(a => a.StudentPoints);

        foreach (DetailedLessonProgress lesson in detailedProgress.Value.UnitProgresses.SelectMany(a => a.LessonProgresses))
        {
            lesson.TotalLessonStudentScore = studentActivities.Where(a => a.LessonId == lesson.LessonId)
                                                              .Sum(a => a.StudentPoints);
        }

        return detailedProgress;
    }
}
