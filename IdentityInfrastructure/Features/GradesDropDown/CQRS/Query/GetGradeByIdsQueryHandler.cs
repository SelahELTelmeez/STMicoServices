using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Grades;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.GradesDropDown.CQRS.Query
{
    public class GetGradeByIdsQueryHandler : IRequestHandler<GetGradeByIdsQuery, CommitResults<GradeResponse>>
    {
        private readonly STIdentityDbContext _dbContext;

        public GetGradeByIdsQueryHandler(STIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommitResults<GradeResponse>> Handle(GetGradeByIdsQuery request, CancellationToken cancellationToken)
        {
            return new CommitResults<GradeResponse>
            {
                ResultType = ResultType.Ok,
                Value = await _dbContext.Set<Grade>().Where(a => a.IsEnabled && request.GradeIds.Contains(a.Id)).ProjectToType<GradeResponse>().ToListAsync(cancellationToken)
            };
        }
    }
}
