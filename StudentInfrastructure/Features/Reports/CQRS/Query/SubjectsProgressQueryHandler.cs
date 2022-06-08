using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Reports.CQRS.Query;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Reports.CQRS.Query
{
    public class SubjectsProgressQueryHandler : IRequestHandler<SubjectsProgressQuery, ICommitResults<SubjectBriefProgressResponse>>
    {
        private readonly CurriculumClient _curriculumClient;
        private readonly StudentDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public SubjectsProgressQueryHandler(CurriculumClient curriculumClient, StudentDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _curriculumClient = curriculumClient;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<ICommitResults<SubjectBriefProgressResponse>> Handle(SubjectsProgressQuery request, CancellationToken cancellationToken)
        {
            ICommitResults<SubjectBriefProgressResponse>? detailedProgress = await _curriculumClient.SubjectsBriefProgressAsync(request.Term, request.StudentId, cancellationToken);

            if (!detailedProgress.IsSuccess)
            {
                return detailedProgress;
            }

            if (detailedProgress.Value.Any())
            {

                IEnumerable<ActivityTracker> studentActivities = await _dbContext.Set<ActivityTracker>()
                                                                                 .Where(a => detailedProgress.Value.Select(a => a.SubjectId).Contains(a.SubjectId)
                                                                                 && a.StudentId == (request.StudentId ?? _httpContextAccessor.GetIdentityUserId())
                                                                                 && a.IsActive == true)
                                                                                 .ToListAsync(cancellationToken);


                foreach (SubjectBriefProgressResponse progressResponse in detailedProgress.Value)
                {
                    var response = studentActivities?.Where(a => a.SubjectId == progressResponse.SubjectId);
                    if (response.Any())
                    {
                        progressResponse.TotalStudentScore = response?.Sum(a => a.StudentPoints) ?? 0;
                    }
                }
            }
            return detailedProgress;
        }
    }
}
