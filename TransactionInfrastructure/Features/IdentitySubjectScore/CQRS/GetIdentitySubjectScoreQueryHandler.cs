using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransactionDomain.Features.IdentitySubjectScore.CQRS;
using TransactionDomain.Features.IdentitySubjectScore.DTO;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Features.IdentitySubjectScore.DTO;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.IdentitySubjectScore.CQRS;

public class GetIdentitySubjectScoreQueryHandler : IRequestHandler<GetIdentitySubjectScoreQuery, CommitResult<IdentitySubjectScoreResponse>>
{
    private readonly HttpClient _CurriculumClient;
    private readonly StudentTrackerDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    public GetIdentitySubjectScoreQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, StudentTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _CurriculumClient = factory.CreateClient("CurriculumClient");
        _CurriculumClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _CurriculumClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResult<IdentitySubjectScoreResponse>> Handle(GetIdentitySubjectScoreQuery request, CancellationToken cancellationToken)
    {
        //TODO: Subject Id => Curriculum service => get all lessons 
        CommitResults<LessonResponse>? LessonResponseResponses = await _CurriculumClient.GetFromJsonAsync<CommitResults<LessonResponse>>($"/Curriculum/GetSubjectLessonScores?SubjectId={request.SubjectId}", cancellationToken);

        if (!LessonResponseResponses.IsSuccess)
            return new CommitResult<IdentitySubjectScoreResponse>
            {
                ErrorCode = LessonResponseResponses.ErrorCode,
                ErrorMessage = LessonResponseResponses.ErrorMessage,
                ResultType = LessonResponseResponses.ResultType
            };

        // TODO: then i will filter the lessons in the StudentLessonTracker
        return new CommitResult<IdentitySubjectScoreResponse>
        {
            ResultType = ResultType.Ok,
            Value = new IdentitySubjectScoreResponse
            {
                SubjectScore = LessonResponseResponses.Value.Sum(a => a.Ponits).GetValueOrDefault(),
                StudentScore = await _dbContext.Set<StudentLessonTracker>()
                                               .Where(a => LessonResponseResponses.Value.Select(a => a.Id).Contains(a.LessonId) && a.StudentId.Equals(_httpContextAccessor.GetIdentityUserId()))
                                               .SumAsync(a => a.StudentPoints, cancellationToken)
            }
        };
    }
}

// Total Lesson Score (Total score for all inclued clips) Progress for the current Student
// Score for each clip for the current student.