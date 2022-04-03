using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
using TransactionDomain.Features.IdentitySubjectScore.CQRS;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.IdentitySubjectScore.CQRS;

public class GetIdentitySubjectScoreQueryHandler : IRequestHandler<GetIdentitySubjectScoreQuery, CommitResult<float>>
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
    public async Task<CommitResult<float>> Handle(GetIdentitySubjectScoreQuery request, CancellationToken cancellationToken)
    {
        //TODO: Subject Id => Curriculum service => get all lessons 
        // TODO: then i will filter the lessons in the StudentLessonTracker
        return new CommitResult<float>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<StudentLessonTracker>().Where(a => a.StudentId.Equals(_httpContextAccessor.GetIdentityUserId())).SumAsync(a => a.StudentPoints, cancellationToken)
        };
    }
}
