using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.DTO;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentDomain.Features.Tracker.DTO;
using StudentEntities.Entities;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;
using StudentInfrastructure.Utilities;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query;
public class GetStudentRecentLessonsProgressQueryHandler : IRequestHandler<GetStudentRecentLessonsProgressQuery, CommitResults<StudentRecentLessonProgressResponse>>
{
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly CurriculumClient _CurriculumClient;
    public GetStudentRecentLessonsProgressQueryHandler(CurriculumClient curriculumClient, IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = curriculumClient;
    }
    public async Task<CommitResults<StudentRecentLessonProgressResponse>> Handle(GetStudentRecentLessonsProgressQuery request, CancellationToken cancellationToken)
    {
        // Read all User's activity 
        List<int> activityRecords = await _dbContext.Set<StudentActivityTracker>()
                                                    .Where(a => a.StudentId.Equals(_userId))
                                                    .OrderByDescending(a => a.CreatedOn)
                                                    .GroupBy(a => a.LessonId)
                                                    .Select(a => a.Key)
                                                    .Take(2)
                                                    .ToListAsync(cancellationToken);

        if (activityRecords.Any())
        {
            CommitResults<LessonBriefResponse>? lessonBreifs = await _CurriculumClient.GetLessonsBriefAsync(activityRecords, cancellationToken);
            if (lessonBreifs.IsSuccess)
            {
                IEnumerable<StudentRecentLessonProgressResponse> Mapper()
                {
                    foreach (LessonBriefResponse briefResponse in lessonBreifs.Value)
                    {
                        yield return new StudentRecentLessonProgressResponse
                        {
                            LessonName = briefResponse.Name,
                            LessonPoints = briefResponse.Ponits.GetValueOrDefault(),
                            StudentPoints = _dbContext.Set<StudentActivityTracker>().Where(a => a.LessonId.Equals(briefResponse.Id)).Sum(a => a.StudentPoints)
                        };
                    }
                    yield break;
                }
                return new CommitResults<StudentRecentLessonProgressResponse>
                {
                    ResultType = ResultType.Ok,
                    Value = Mapper()
                };
            }

        }
        return new CommitResults<StudentRecentLessonProgressResponse>
        {
            ResultType = ResultType.Ok,
            Value = default
        };
    }
}