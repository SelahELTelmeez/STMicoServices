using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.TeacherClass.DTO.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetStudentClassesQueryHandler : IRequestHandler<GetStudentClassesQuery, CommitResults<StudentClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IHttpContextAccessor? _httpContextAccessor;

    public GetStudentClassesQueryHandler(IHttpContextAccessor httpContextAccessor, TeacherDbContext dbContext)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
    }
    public async Task<CommitResults<StudentClassResponse>> Handle(GetStudentClassesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ClassEnrollee> classEnrollees = await _dbContext.Set<ClassEnrollee>()
                       .Where(a => a.StudentId.Equals(request.StudentId ?? _httpContextAccessor.GetIdentityUserId()) && a.IsActive)
                       .Include(a => a.TeacherClassFK)
                       .ToListAsync(cancellationToken);

        IEnumerable<StudentClassResponse> Mapper()
        {
            foreach (TeacherClass teacherClass in classEnrollees.Select(a => a.TeacherClassFK))
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
