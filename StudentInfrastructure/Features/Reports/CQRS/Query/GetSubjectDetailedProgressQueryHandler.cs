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
            detailedProgress.ResultType.GetValueCommitResult<DetailedProgressResponse>(default, detailedProgress.ErrorCode, detailedProgress.ErrorMessage);
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
                var maxedActivity = response.MaxBy(a => a.StudentPoints);
                lesson.TotalLessonStudentScore = maxedActivity?.StudentPoints ?? 0;
                lesson.ActivityDate = maxedActivity.CreatedOn.GetValueOrDefault();
            }
        }

        detailedProgress.Value.TotalSubjectStudentScore = detailedProgress.Value.UnitProgresses.SelectMany(a => a.LessonProgresses).Sum(a => a.TotalLessonStudentScore);

        detailedProgress.Value.UnitProgresses.SelectMany(a => a.LessonProgresses).OrderBy(a => a.ActivityDate);


        IEnumerable<DetailedUnitProgress> Mapper()
        {
            foreach (var unit in detailedProgress.Value.UnitProgresses)
            {
                yield return new DetailedUnitProgress
                {
                    UnitId = unit.UnitId,
                    UnitName = unit.UnitName,
                    LessonProgresses = unit.LessonProgresses.OrderBy(a => a.ActivityDate)
                };
            }
        }

        return ResultType.Ok.GetValueCommitResult<DetailedProgressResponse>(new DetailedProgressResponse
        {
            SubjectId = detailedProgress.Value.SubjectId,
            SubjectName = detailedProgress.Value.SubjectName,
            TotalSubjectScore = detailedProgress.Value.TotalSubjectScore,
            TotalSubjectStudentScore = detailedProgress.Value.TotalSubjectStudentScore,
            UnitProgresses = Mapper()
        });
    }
}
