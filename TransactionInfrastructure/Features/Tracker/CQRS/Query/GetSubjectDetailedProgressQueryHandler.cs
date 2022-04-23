using Mapster;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Tracker.CQRS.Query;
using TransactionDomain.Features.Tracker.DTO.Query;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.HttpClients;

namespace TransactionInfrastructure.Features.Tracker.CQRS.Query;

public class GetSubjectDetailedProgressQueryHandler : IRequestHandler<GetSubjectDetailedProgressQuery, CommitResult<DetailedProgressResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly CurriculumClient _curriculumClient;
    public GetSubjectDetailedProgressQueryHandler(TrackerDbContext dbContext, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _curriculumClient = curriculumClient;
    }
    public async Task<CommitResult<DetailedProgressResponse>> Handle(GetSubjectDetailedProgressQuery request, CancellationToken cancellationToken)
    {
        CommitResult<DetailedProgressResponse>? detailedProgress = await _curriculumClient.GetSubjectDetailedProgressAsync(request.SubjectId, cancellationToken);
        if (!detailedProgress.IsSuccess)
        {
            return detailedProgress.Adapt<CommitResult<DetailedProgressResponse>>();
        }

        IEnumerable<StudentActivityTracker> studentActivities = await _dbContext.Set<StudentActivityTracker>()
                                                                                .Where(a => a.SubjectId == request.SubjectId)
                                                                                .ToListAsync(cancellationToken);


        detailedProgress.Value.TotalSubjectStudentScore = studentActivities.Where(a => a.SubjectId == request.SubjectId)
                                                                           .Sum(a => a.StudentPoints);

        foreach (DetailedLessonProgress lesson in detailedProgress.Value.UnitProgresses.SelectMany(a => a.LessonProgresses))
        {
            lesson.TotalLessonStudentScore = studentActivities.Where(a => a.LessonId == lesson.LessonId)
                                                              .Sum(a => a.StudentPoints);
        }

        return detailedProgress;
    }
}
