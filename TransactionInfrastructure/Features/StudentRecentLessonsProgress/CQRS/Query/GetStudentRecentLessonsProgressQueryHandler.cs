using CurriculumDomain.Features.LessonDetails.DTO.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransactionDomain.Features.GetStudentRecentLessonsProgress.DTO;
using TransactionDomain.Features.StudentRecentLessonsProgress.CQRS.Query;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.StudentRecentLessonsProgress.CQRS.Query;

public class GetStudentRecentLessonsProgressQueryHandler : IRequestHandler<GetStudentRecentLessonsProgressQuery, CommitResult<List<StudentRecentLessonProgressResponse>>>
{
    private readonly StudentTrackerDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly HttpClient _CurriculumClient;
    public GetStudentRecentLessonsProgressQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, StudentTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = factory.CreateClient("CurriculumClient");
        _CurriculumClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _CurriculumClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());

    }
    public async Task<CommitResult<List<StudentRecentLessonProgressResponse>>> Handle(GetStudentRecentLessonsProgressQuery request, CancellationToken cancellationToken)
    {
        // Read all User's activity 
        List<int> activityRecords = await _dbContext.Set<StudentLessonTracker>()
                                                                      .Where(a => a.StudentId.Equals(_userId))
                                                                      .OrderByDescending(a => a.CreatedOn)
                                                                      .GroupBy(a => a.LessonId)
                                                                      .Select(a => a.Key)
                                                                      .Take(2)
                                                                      .ToListAsync(cancellationToken);

        if (activityRecords.Count == 1)
        {
            StudentLessonTracker? firstActivityRecord = await _dbContext.Set<StudentLessonTracker>().SingleOrDefaultAsync(a => a.Id.Equals(activityRecords[0]), cancellationToken);

            CommitResult<LessonDetailsReponse>? firstLessonDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<LessonDetailsReponse>>($"/Curriculum/GetLessonDetails?LessonId={firstActivityRecord.LessonId}");

            return new CommitResult<List<StudentRecentLessonProgressResponse>>
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
            StudentLessonTracker? firstActivityRecord = await _dbContext.Set<StudentLessonTracker>().SingleOrDefaultAsync(a => a.LessonId.Equals(activityRecords[0]), cancellationToken: cancellationToken);
            StudentLessonTracker? secondActivityRecord = await _dbContext.Set<StudentLessonTracker>().SingleOrDefaultAsync(a => a.LessonId.Equals(activityRecords[1]), cancellationToken: cancellationToken);

            CommitResult<LessonDetailsReponse>? firstLessonDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<LessonDetailsReponse>>($"/Curriculum/GetLessonDetails?LessonId={firstActivityRecord.LessonId}");
            CommitResult<LessonDetailsReponse>? secondLessonDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<LessonDetailsReponse>>($"/Curriculum/GetLessonDetails?LessonId={secondActivityRecord.LessonId}");


            return new CommitResult<List<StudentRecentLessonProgressResponse>>
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
        return new CommitResult<List<StudentRecentLessonProgressResponse>>
        {
            ResultType = ResultType.Ok,
            Value = new List<StudentRecentLessonProgressResponse>()
        };
    }
}