using SharedModule.DTO;
using StudentDomain.Features.Reports.CQRS.Query;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Reports.CQRS.Query
{
    public class SubjectsProgressQueryHandler : IRequestHandler<SubjectsProgressQuery, CommitResults<SubjectBriefProgressResponse>>
    {
        private readonly CurriculumClient _curriculumClient;
        private readonly StudentDbContext _dbContext;
        public SubjectsProgressQueryHandler(CurriculumClient curriculumClient, StudentDbContext dbContext)
        {
            _curriculumClient = curriculumClient;
            _dbContext = dbContext;
        }
        public async Task<CommitResults<SubjectBriefProgressResponse>> Handle(SubjectsProgressQuery request, CancellationToken cancellationToken)
        {
            CommitResults<SubjectBriefProgressResponse>? detailedProgress = await _curriculumClient.SubjectsBriefProgressAsync(request.Term, cancellationToken);

            if (!detailedProgress.IsSuccess)
            {
                return detailedProgress;
            }

            if (detailedProgress.Value.Any())
            {

                IEnumerable<ActivityTracker> studentActivities = await _dbContext.Set<ActivityTracker>()
                                                                                 .Where(a => detailedProgress.Value.Select(a => a.SubjectId).Contains(a.SubjectId))
                                                                                 .ToListAsync(cancellationToken);


                foreach (SubjectBriefProgressResponse progressResponse in detailedProgress.Value)
                {
                    progressResponse.TotalStudentScore = studentActivities.Where(a => a.SubjectId == progressResponse.SubjectId)
                                                                       .Sum(a => a.StudentPoints);

                }
            }
            return detailedProgress;
        }
    }
}
