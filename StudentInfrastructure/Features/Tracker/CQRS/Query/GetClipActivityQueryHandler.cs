using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentInfrastructure.HttpClients;
using DomainEntities = StudentEntities.Entities.Trackers;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query;
public class GetClipActivityQueryHandler : IRequestHandler<GetClipActivityQuery, ICommitResults<ClipActivityResponse>>
{
    private readonly StudentDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly Guid? _UserId;
    public GetClipActivityQueryHandler(IHttpContextAccessor httpContextAccessor, StudentDbContext dbContext, IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _UserId = httpContextAccessor.GetIdentityUserId();
        _identityClient = identityClient;
    }
    public async Task<ICommitResults<ClipActivityResponse>> Handle(GetClipActivityQuery request, CancellationToken cancellationToken)
    {
        ICommitResult<int>? currentStudentGrade = await _identityClient.GetStudentGradeAsync(_UserId, cancellationToken);
        if (!currentStudentGrade.IsSuccess)
        {
            currentStudentGrade.ResultType.GetValueCommitResults(Array.Empty<ClipActivityResponse>(), currentStudentGrade.ErrorCode, currentStudentGrade.ErrorMessage);
        }
        return ResultType.Ok.GetValueCommitResults(await _dbContext.Set<DomainEntities.ActivityTracker>()
                                    .Where(a => request.ClipIds.Contains(a.ClipId) && a.StudentId.Equals(_UserId) && a.GradeId == currentStudentGrade.Value)
                                    .ProjectToType<ClipActivityResponse>()
                                    .ToListAsync(cancellationToken));
    }
}
