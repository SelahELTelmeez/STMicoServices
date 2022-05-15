using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query
{
    public class GetTeachersByStudentIdQueryHandler : IRequestHandler<GetTeachersByStudentIdQuery, CommitResults<LimitedProfileResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly IdentityClient _IdentityClient;
        public GetTeachersByStudentIdQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
        }
        public async Task<CommitResults<LimitedProfileResponse>?> Handle(GetTeachersByStudentIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Guid> TeacherIds = await _dbContext.Set<ClassEnrollee>()
                                                           .Where(a => a.StudentId == request.StudentId && a.IsActive)
                                                           .Include(a => a.TeacherClassFK)
                                                           .Select(a => a.TeacherClassFK.TeacherId)
                                                           .ToListAsync(cancellationToken);
            if (TeacherIds.Any())
            {
                return await _IdentityClient.GetIdentityLimitedProfilesAsync(TeacherIds, cancellationToken);
            }
            else
            {
                return new CommitResults<LimitedProfileResponse>
                {
                    ResultType = ResultType.Empty,
                    Value = Array.Empty<LimitedProfileResponse>()
                };
            }
        }
    }
}
