using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransactionDomain.Features.Tracker.CQRS.Query;
using TransactionDomain.Features.Tracker.DTO;
using TransactionDomain.Features.Tracker.DTO.Query;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Tracker.CQRS.Query;

// TODO: Need more improvments 
public class GetStudentRecentLessonsProgressQueryHandler : IRequestHandler<GetStudentRecentLessonsProgressQuery, CommitResults<StudentRecentLessonProgressResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly HttpClient _CurriculumClient;
    public GetStudentRecentLessonsProgressQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = factory.CreateClient("CurriculumClient");
        _CurriculumClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _CurriculumClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
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

        if (activityRecords.Count == 1)
        {
            StudentActivityTracker? firstActivityRecord = await _dbContext.Set<StudentActivityTracker>().SingleOrDefaultAsync(a => a.LessonId.Equals(activityRecords[0]), cancellationToken);

            CommitResult<LessonDetailsReponse>? firstLessonDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<LessonDetailsReponse>>($"/Curriculum/GetLessonDetails?LessonId={firstActivityRecord.LessonId}");

            return new CommitResults<StudentRecentLessonProgressResponse>
            {
                ResultType = ResultType.Ok,
                Value = new List<StudentRecentLessonProgressResponse>
                {
                   new StudentRecentLessonProgressResponse
                   {
                        LessonName = firstLessonDetails.Value.Name,
                        LessonPoints = firstLessonDetails.Value.Ponits.GetValueOrDefault(),
                        StudentPoints = firstActivityRecord.StudentPoints
                   },
                },
            };
        }
        if (activityRecords.Count == 2)
        {
            StudentActivityTracker? firstActivityRecord = await _dbContext.Set<StudentActivityTracker>().SingleOrDefaultAsync(a => a.LessonId.Equals(activityRecords[0]), cancellationToken: cancellationToken);
            StudentActivityTracker? secondActivityRecord = await _dbContext.Set<StudentActivityTracker>().SingleOrDefaultAsync(a => a.LessonId.Equals(activityRecords[1]), cancellationToken: cancellationToken);

            CommitResult<LessonDetailsReponse>? firstLessonDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<LessonDetailsReponse>>($"/Curriculum/GetLessonDetails?LessonId={firstActivityRecord.LessonId}");
            CommitResult<LessonDetailsReponse>? secondLessonDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<LessonDetailsReponse>>($"/Curriculum/GetLessonDetails?LessonId={secondActivityRecord.LessonId}");


            return new CommitResults<StudentRecentLessonProgressResponse>
            {
                ResultType = ResultType.Ok,
                Value = new List<StudentRecentLessonProgressResponse>
                {
                   new StudentRecentLessonProgressResponse
                   {
                        LessonName = firstLessonDetails.Value.Name,
                        LessonPoints = firstLessonDetails.Value.Ponits.GetValueOrDefault(),
                        StudentPoints = firstActivityRecord.StudentPoints,
                   },
                   new StudentRecentLessonProgressResponse
                   {
                        LessonName = secondLessonDetails.Value.Name,
                        LessonPoints = secondLessonDetails.Value.Ponits.GetValueOrDefault(),
                        StudentPoints = secondActivityRecord.StudentPoints
                   }
                },
            };
        }
        return new CommitResults<StudentRecentLessonProgressResponse>
        {
            ResultType = ResultType.Ok,
            Value = new List<StudentRecentLessonProgressResponse>()
        };
    }
}