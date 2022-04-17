using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Assignment.CQRS.Query;
using TransactionDomain.Features.Assignment.DTO.Query;
using TransactionEntites.Entities;
using TransactionEntites.Entities.TeacherActivity;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Assignment.CQRS.Query
{
    public class GetAssignmentQueryHandler : IRequestHandler<GetAssignmentQuery, CommitResults<AssignmentResponse>>
    {
        private readonly TrackerDbContext _dbContext;
        private readonly Guid? _userId;
        public GetAssignmentQueryHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }

        public async Task<CommitResults<AssignmentResponse>> Handle(GetAssignmentQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TeacherAssignment> teacherAssignments = await _dbContext.Set<TeacherAssignment>()
                                                                                .Where(a => a.Creator.Equals(_userId))
                                                                                .Include(a => a.TeacherClasses)
                                                                                .ThenInclude(a => a.StudentEnrolls)
                                                                                .ToListAsync(cancellationToken);

            IEnumerable<AssignmentResponse> Mapper()
            {
                foreach (TeacherAssignment assignment in teacherAssignments)
                {
                    yield return new AssignmentResponse
                    {
                        Description = assignment.Description,
                        CreatedOn = assignment.CreatedOn.GetValueOrDefault(),
                        EndDate = assignment.EndDate,
                        Id = assignment.Id,
                        Title = assignment.Title,
                        EntrolledCounter = assignment.TeacherClasses.SelectMany(a => a.StudentEnrolls).Where(a => a.IsActive).Count(),
                    };
                }
                yield break;
            }

            return new CommitResults<AssignmentResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
