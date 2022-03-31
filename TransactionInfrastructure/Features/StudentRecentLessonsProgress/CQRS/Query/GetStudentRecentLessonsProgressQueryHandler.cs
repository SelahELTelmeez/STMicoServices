using CurriculumDomain.Features.LessonDetails.DTO.Query;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
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
    }
    public async Task<CommitResult<List<StudentRecentLessonProgressResponse>>> Handle(GetStudentRecentLessonsProgressQuery request, CancellationToken cancellationToken)
    {
        // Read all User's activity 
        List<int> activityRecords = await _dbContext.Set<StudentLessonTracker>()
                                                                      .Where(a => a.StudentId.Equals(_userId))
                                                                      .OrderByDescending(a => a.CreatedOn)
                                                                      .GroupBy(a => a.Id)
                                                                      .Select(a => a.Key)
                                                                      .Take(2)
                                                                      .ToListAsync(cancellationToken);

        if (activityRecords.Count == 1)
        {
            StudentLessonTracker? firstActivityRecord = await _dbContext.Set<StudentLessonTracker>().SingleOrDefaultAsync(a => a.Id.Equals(activityRecords[0]), cancellationToken);

            LessonDetailsReponse? firstLessonDetails = await _CurriculumClient.GetFromJsonAsync<LessonDetailsReponse>($"/Curriculum/GetLessonDetails?LessonId={firstActivityRecord.LessonId}");

            return new CommitResult<List<StudentRecentLessonProgressResponse>>
            {
                ResultType = ResultType.Ok,
                Value = new List<StudentRecentLessonProgressResponse>
                {
                   new StudentRecentLessonProgressResponse
                   {
                        LessonName = firstLessonDetails.Name,
                        LessonPoints = firstLessonDetails.Ponits.GetValueOrDefault(),
                        StudentPoints = firstActivityRecord.StudentPoints
                   },
                },
            };
        }
        if (activityRecords.Count == 2)
        {
            StudentLessonTracker? firstActivityRecord = await _dbContext.Set<StudentLessonTracker>().SingleOrDefaultAsync(a => a.Id.Equals(activityRecords[0]), cancellationToken: cancellationToken);
            StudentLessonTracker? secondActivityRecord = await _dbContext.Set<StudentLessonTracker>().SingleOrDefaultAsync(a => a.Id.Equals(activityRecords[1]), cancellationToken: cancellationToken);

            LessonDetailsReponse? firstLessonDetails = await _CurriculumClient.GetFromJsonAsync<LessonDetailsReponse>($"/Curriculum/GetLessonDetails?LessonId={firstActivityRecord.LessonId}");
            LessonDetailsReponse? secondLessonDetails = await _CurriculumClient.GetFromJsonAsync<LessonDetailsReponse>($"/Curriculum/GetLessonDetails?LessonId={secondActivityRecord.LessonId}");


            return new CommitResult<List<StudentRecentLessonProgressResponse>>
            {
                ResultType = ResultType.Ok,
                Value = new List<StudentRecentLessonProgressResponse>
                {
                   new StudentRecentLessonProgressResponse
                   {
                        LessonName = firstLessonDetails.Name,
                        LessonPoints = firstLessonDetails.Ponits.GetValueOrDefault(),
                        StudentPoints = firstActivityRecord.StudentPoints,
                   },
                   new StudentRecentLessonProgressResponse
                   {
                        LessonName = secondLessonDetails.Name,
                        LessonPoints = secondLessonDetails.Ponits.GetValueOrDefault(),
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