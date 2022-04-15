using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.TeacherSubject.CQRS.Command;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomianEntities = TransactionEntites.Entities.TeacherSubjects;
namespace TransactionInfrastructure.Features.TeacherSubject.CQRS.Command;

public class AddTeacherSubjectCommandHandler : IRequestHandler<AddTeacherSubjectCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;

    public AddTeacherSubjectCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<CommitResult> Handle(AddTeacherSubjectCommand request, CancellationToken cancellationToken)
    {
        IEnumerable<DomianEntities.TeacherSubject>? teacherSubjects = await _dbContext.Set<DomianEntities.TeacherSubject>()
                .Where(a => a.TeacherId.Equals(_userId))
                .ToListAsync(cancellationToken);

        if (teacherSubjects.Any())
        {
            _dbContext.Set<DomianEntities.TeacherSubject>().RemoveRange(teacherSubjects);
        }
        else
        {
            foreach (string SubjectId in request.SubjectIds)
            {
                _dbContext.Set<DomianEntities.TeacherSubject>().Add(new DomianEntities.TeacherSubject
                {
                    SubjectId = SubjectId,
                    TeacherId = _userId.GetValueOrDefault()
                });
            }
        }
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
