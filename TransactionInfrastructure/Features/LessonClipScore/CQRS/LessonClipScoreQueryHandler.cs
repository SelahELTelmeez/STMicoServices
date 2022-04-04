using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransactionDomain.Features.LessonClipScore.CQRS;
using TransactionDomain.Features.LessonClipScore.DTO;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Features.LessonClipScore.DTO;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.LessonClipScore.CQRS;
public class LessonClipScoreQueryHandler : IRequestHandler<LessonClipScoreQuery, CommitResult<LessonClipScoreResponse>>
{
    private readonly HttpClient _CurriculumClient;
    private readonly StudentTrackerDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public LessonClipScoreQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, StudentTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _CurriculumClient = factory.CreateClient("CurriculumClient");
        _CurriculumClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _CurriculumClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResult<LessonClipScoreResponse>> Handle(LessonClipScoreQuery request, CancellationToken cancellationToken)
    {
        //TODO: Lesson Id => Curriculum service => get all Clips that related by Lesson Id
        CommitResult<List<LessonClipResponse>>? LessonClipResponses = await _CurriculumClient.GetFromJsonAsync<CommitResult<List<LessonClipResponse>>>($"/Curriculum/GetLessonClipScores?LessonId={request.LessonId}", cancellationToken);

        if (!LessonClipResponses.IsSuccess)
            return new CommitResult<LessonClipScoreResponse>
            {
                ErrorCode = LessonClipResponses.ErrorCode,
                ErrorMessage = LessonClipResponses.ErrorMessage,
                ResultType = LessonClipResponses.ResultType
            };


        // TODO: then i will filter the lessons in the StudentLessonTracker
        return new CommitResult<LessonClipScoreResponse>
        {
            ResultType = ResultType.Ok,
            Value = new LessonClipScoreResponse
            {
                LessonScore = LessonClipResponses.Value.Sum(a => a.Ponits).GetValueOrDefault(),
                StudentScore = await _dbContext.Set<StudentActivityTracker>()
                                      .Where(a => LessonClipResponses.Value.Select(a => a.Id).Contains(a.ClipId) && a.StudentId.Equals(_httpContextAccessor.GetIdentityUserId()) && a.IsActive)
                                      .GroupBy(a => a.ClipId).Select(g => g.Max(a => a.StudentPoints))
                                      .SumAsync(cancellationToken)
            }
        };
    }
}
