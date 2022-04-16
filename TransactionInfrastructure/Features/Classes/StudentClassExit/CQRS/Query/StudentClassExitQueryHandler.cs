using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.StudentClassExit.CQRS.Query;
using TransactionDomain.Features.Classes.StudentClassExit.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.StudentClassExit.CQRS.Query;
public class StudentClassExitQueryHandler : IRequestHandler<StudentClassExitQuery, CommitResult<StudentClassResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    public StudentClassExitQueryHandler(IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResult<StudentClassResponse>> Handle(StudentClassExitQuery request, CancellationToken cancellationToken)
    {
        DomainEntities.StudentEnrollClass? teacherClass = await _dbContext.Set<DomainEntities.StudentEnrollClass>()
                                      .Where(a => a.StudentId.Equals(_userId) && a.ClassId.Equals(request.ClassId))
                                      .SingleOrDefaultAsync(cancellationToken);

        return new CommitResult<StudentClassResponse>
        {
            ResultType = ResultType.Ok,
            Value = teacherClass?.Adapt<StudentClassResponse>()
        };
    }
}