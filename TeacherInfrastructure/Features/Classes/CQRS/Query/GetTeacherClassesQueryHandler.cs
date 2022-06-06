using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.TeacherClass.DTO.Query;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query
{
    public class GetTeacherClassesQueryHandler : IRequestHandler<GetTeacherClassesQuery, ICommitResults<TeacherClassResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly Guid? _userId;

        public GetTeacherClassesQueryHandler(IHttpContextAccessor httpContextAccessor, TeacherDbContext dbContext)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<ICommitResults<TeacherClassResponse>> Handle(GetTeacherClassesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                       .Where(a => a.TeacherId.Equals(_userId) && a.IsActive)
                                                                       .Include(a => a.ClassEnrollees)
                                                                       .ToListAsync(cancellationToken);

            IEnumerable<TeacherClassResponse> Mapper()
            {
                foreach (TeacherClass teacherClass in teacherClasses)
                {
                    yield return new TeacherClassResponse
                    {
                        Description = teacherClass.Description,
                        SubjectId = teacherClass.SubjectId,
                        Name = teacherClass.Name,
                        IsActive = teacherClass.IsActive,
                        Id = teacherClass.Id,
                        EnrollersCount = teacherClass.ClassEnrollees.Where(a => a.IsActive).Count(),
                    };
                }
                yield break;
            }

            return ResultType.Ok.GetValueCommitResults(Mapper());
        }
    }
}
