using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.TeacherClass.CQRS.Query;
using TransactionDomain.Features.TeacherClass.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;
namespace TransactionInfrastructure.Features.TeacherClass.Query
{
    public class GetTeacherClassesBySubjectQueryHandler : IRequestHandler<GetTeacherClassesBySubjectQuery, CommitResults<TeacherClassResponse>>
    {
        private readonly TrackerDbContext _dbContext;
        private readonly Guid? _userId;

        public GetTeacherClassesBySubjectQueryHandler(IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }
        public async Task<CommitResults<TeacherClassResponse>> Handle(GetTeacherClassesBySubjectQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<DomainEntities.TeacherClass> teacherClasses = await _dbContext.Set<DomainEntities.TeacherClass>()
                .Where(a => a.SubjectId.Equals(request.SubjectId) && a.TeacherId.Equals(_userId))
                .Include(a => a.StudentEnrolls)
                .ToListAsync(cancellationToken);

            IEnumerable<TeacherClassResponse> Mapper()
            {
                foreach (DomainEntities.TeacherClass teacherClass in teacherClasses)
                {
                    yield return new TeacherClassResponse
                    {
                        Description = teacherClass.Description,
                        SubjectId = teacherClass.SubjectId,
                        Name = teacherClass.Name,
                        IsActive = teacherClass.IsActive,
                        Id = teacherClass.Id,
                        EntrollersCount = teacherClass.StudentEnrolls.Where(a => a.IsActive).Count(),
                    };
                }
                yield break;
            }

            return new CommitResults<TeacherClassResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
