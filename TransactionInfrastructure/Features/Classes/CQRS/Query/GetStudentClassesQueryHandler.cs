using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Query;
using TransactionDomain.Features.TeacherClass.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.CQRS.Query;

public class GetStudentClassesQueryHandler : IRequestHandler<GetStudentClassesQuery, CommitResults<StudentClassResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;

    public GetStudentClassesQueryHandler(IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<StudentClassResponse>> Handle(GetStudentClassesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<DomainEntities.StudentEnrollClass> studentEnrollClasses = await _dbContext.Set<DomainEntities.StudentEnrollClass>()
                       .Where(a => a.StudentId.Equals(_userId) && a.IsActive)
                       .Include(a => a.TeacherClassFK)
                       .ToListAsync(cancellationToken);

        IEnumerable<StudentClassResponse> Mapper()
        {
            foreach (DomainEntities.TeacherClass teacherClass in studentEnrollClasses.Select(a => a.TeacherClassFK))
            {
                yield return new StudentClassResponse
                {
                    Description = teacherClass.Description,
                    SubjectId = teacherClass.SubjectId,
                    Name = teacherClass.Name,
                    IsActive = teacherClass.IsActive,
                    Id = teacherClass.Id,
                };
            }
            yield break;
        }

        return new CommitResults<StudentClassResponse>
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };
    }
}
