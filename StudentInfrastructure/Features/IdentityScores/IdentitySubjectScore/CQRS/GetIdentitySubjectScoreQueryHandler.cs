using SharedModule.Extensions;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.CQRS;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.DTO;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.IdentitySubjectScore.IdentitySubjectScore.CQRS;
public class GetIdentitySubjectScoreQueryHandler : IRequestHandler<GetIdentitySubjectScoreQuery, CommitResult<IdentitySubjectScoreResponse>>
{
    private readonly CurriculumClient _CurriculumClient;
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _userId;
    public GetIdentitySubjectScoreQueryHandler(CurriculumClient curriculumClient, IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = curriculumClient;
    }
    public async Task<CommitResult<IdentitySubjectScoreResponse>> Handle(GetIdentitySubjectScoreQuery request, CancellationToken cancellationToken)
    {
        //TODO: Subject Id => Curriculum service => get all lessons 
        CommitResults<LessonBriefResponse>? lessonBriefResponse = await _CurriculumClient.GetLessonsBriefBySubjectAsync(request.SubjectId, cancellationToken);

        if (!lessonBriefResponse.IsSuccess)
            return lessonBriefResponse.Adapt<CommitResult<IdentitySubjectScoreResponse>>();


        // TODO: then i will filter the lessons in the StudentLessonTracker
        return new CommitResult<IdentitySubjectScoreResponse>
        {
            ResultType = ResultType.Ok,
            Value = new IdentitySubjectScoreResponse
            {
                SubjectScore = lessonBriefResponse.Value.Sum(a => a.Ponits).GetValueOrDefault(),
                StudentScore = await _dbContext.Set<StudentActivityTracker>()
                                               .Where(a => lessonBriefResponse.Value.Select(a => a.Id).Contains(a.LessonId) && a.StudentId.Equals(_userId))
                                               .SumAsync(a => a.StudentPoints, cancellationToken)
            }
        };
    }
}