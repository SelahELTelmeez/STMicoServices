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

        if (activityRecords.Any())
        {
            HttpResponseMessage httpResponse = await _CurriculumClient.PostAsJsonAsync($"/Curriculum/GetLessonsBrief", activityRecords, cancellationToken);
            if (httpResponse.IsSuccessStatusCode)
            {
                CommitResults<LessonBriefResponse>? lessonBreifs = await httpResponse.Content.ReadFromJsonAsync<CommitResults<LessonBriefResponse>>(cancellationToken: cancellationToken);

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