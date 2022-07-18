using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.IdentityScores.IdentityClipScore.CQRS.Query;
using StudentDomain.Features.IdentityScores.IdentityClipScore.DTO;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.IdentityScores.IdentityClipScore.CQRS;
public class GetIdentityClipsScoreQueryHandler : IRequestHandler<GetIdentityClipsScoreQuery, ICommitResult<IdentityClipsScoreResponse>>
{
    private readonly CurriculumClient _CurriculumClient;
    private readonly StudentDbContext _dbContext;
    private readonly string? _userId;
    public GetIdentityClipsScoreQueryHandler(CurriculumClient curriculumClient, IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = curriculumClient;
    }
    public async Task<ICommitResult<IdentityClipsScoreResponse>> Handle(GetIdentityClipsScoreQuery request, CancellationToken cancellationToken)
    {
        //TODO: Lesson Id => Curriculum service => get all Clips that related by Lesson Id
        ICommitResults<ClipBriefResponse>? clipBriefResponse = await _CurriculumClient.GetClipsBriefAsync(request.LessonId, cancellationToken);

        if (!clipBriefResponse.IsSuccess)
        {
            clipBriefResponse.ResultType.GetValueCommitResult<IdentityClipsScoreResponse>(default, clipBriefResponse.ErrorCode, clipBriefResponse.ErrorMessage);
        }
        return ResultType.Ok.GetValueCommitResult(new IdentityClipsScoreResponse
        {
            LessonScore = clipBriefResponse.Value.Sum(a => a.Ponits).GetValueOrDefault(),
            StudentScore = await _dbContext.Set<ActivityTracker>()
                                      .Where(a => clipBriefResponse.Value.Select(a => a.Id).Contains(a.ClipId) && a.StudentId.Equals(_userId))
                                      .SumAsync(a => a.StudentPoints, cancellationToken)
        });
    }
}
