using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.IdentityScores.IdentityClipScore.CQRS.Query;
using StudentDomain.Features.IdentityScores.IdentityClipScore.DTO;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.IdentityScores.IdentityClipScore.CQRS;
public class GetIdentityClipsScoreQueryHandler : IRequestHandler<GetIdentityClipsScoreQuery, CommitResult<IdentityClipsScoreResponse>>
{
    private readonly CurriculumClient _CurriculumClient;
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _userId;
    public GetIdentityClipsScoreQueryHandler(CurriculumClient curriculumClient, IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = curriculumClient;
    }
    public async Task<CommitResult<IdentityClipsScoreResponse>> Handle(GetIdentityClipsScoreQuery request, CancellationToken cancellationToken)
    {
        //TODO: Lesson Id => Curriculum service => get all Clips that related by Lesson Id
        CommitResults<ClipBriefResponse>? clipBriefResponse = await _CurriculumClient.GetClipsBriefAsync(request.LessonId, cancellationToken);

        if (!clipBriefResponse.IsSuccess)
        {
            return new CommitResult<IdentityClipsScoreResponse>
            {
                ErrorCode = clipBriefResponse.ErrorCode,
                ErrorMessage = clipBriefResponse.ErrorMessage,
                ResultType = clipBriefResponse.ResultType
            };
        }

        return new CommitResult<IdentityClipsScoreResponse>
        {
            ResultType = ResultType.Ok,
            Value = new IdentityClipsScoreResponse
            {
                LessonScore = clipBriefResponse.Value.Sum(a => a.Ponits).GetValueOrDefault(),
                StudentScore = await _dbContext.Set<ActivityTracker>()
                                      .Where(a => clipBriefResponse.Value.Select(a => a.Id).Contains(a.ClipId) && a.StudentId.Equals(_userId))
                                      .SumAsync(a => a.StudentPoints, cancellationToken)
            }
        };
    }
}
