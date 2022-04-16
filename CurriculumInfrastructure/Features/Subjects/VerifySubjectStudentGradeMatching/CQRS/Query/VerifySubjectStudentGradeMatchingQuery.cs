using CurriculumDomain.Features.Subjects.VerifySubjectStudentGradeMatching.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Subjects.VerifySubjectStudentGradeMatching.CQRS.Query;

public class VerifySubjectStudentGradeMatchingQueryHandler : IRequestHandler<VerifySubjectStudentGradeMatchingQuery, CommitResult<bool>>
{
    private readonly CurriculumDbContext _dbContext;

    public VerifySubjectStudentGradeMatchingQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommitResult<bool>> Handle(VerifySubjectStudentGradeMatchingQuery request, CancellationToken cancellationToken)
    {
        Subject? subject = await _dbContext.Set<Subject>().SingleOrDefaultAsync(a => a.Id.Equals(request.SubjectId) && a.Grade.Equals(request.GradeId), cancellationToken);
        return new CommitResult<bool>
        {
            ResultType = ResultType.Ok,
            Value = subject != null
        };
    }
}


