using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransactionDomain.Features.IdentityScores.IdentityClipScore.CQRS.Query;
using TransactionDomain.Features.IdentityScores.IdentityClipScore.DTO;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.IdentityScores.IdentityClipScore.CQRS;
public class GetIdentityClipsScoreQueryHandler : IRequestHandler<GetIdentityClipsScoreQuery, CommitResult<IdentityClipsScoreResponse>>
{
    private readonly HttpClient _CurriculumClient;
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    public GetIdentityClipsScoreQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = factory.CreateClient("CurriculumClient");
        _CurriculumClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _CurriculumClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResult<IdentityClipsScoreResponse>> Handle(GetIdentityClipsScoreQuery request, CancellationToken cancellationToken)
    {
        //TODO: Lesson Id => Curriculum service => get all Clips that related by Lesson Id
        CommitResults<ClipBriefResponse>? clipBriefResponse = await _CurriculumClient.GetFromJsonAsync<CommitResults<ClipBriefResponse>>($"/Curriculum/GetClipsBrief?LessonId={request.LessonId}", cancellationToken);

        if (!clipBriefResponse.IsSuccess)
            return clipBriefResponse.Adapt<CommitResult<IdentityClipsScoreResponse>>();


        return new CommitResult<IdentityClipsScoreResponse>
        {
            ResultType = ResultType.Ok,
            Value = new IdentityClipsScoreResponse
            {
                LessonScore = clipBriefResponse.Value.Sum(a => a.Ponits).GetValueOrDefault(),
                StudentScore = await _dbContext.Set<StudentActivityTracker>()
                                      .Where(a => clipBriefResponse.Value.Select(a => a.Id).Contains(a.ClipId) && a.StudentId.Equals(_userId))
                                      .SumAsync(a => a.StudentPoints, cancellationToken)
            }
        };
    }
}
