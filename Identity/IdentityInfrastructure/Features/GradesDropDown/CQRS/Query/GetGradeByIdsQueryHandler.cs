using Flaminco.CommitResult;
using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Grades;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.GradesDropDown.CQRS.Query
{
    public class GetGradeByIdsQueryHandler : IRequestHandler<GetGradeByIdsQuery, ICommitResults<GradeResponse>>
    {
        private readonly STIdentityDbContext _dbContext;

        public GetGradeByIdsQueryHandler(STIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ICommitResults<GradeResponse>> Handle(GetGradeByIdsQuery request, CancellationToken cancellationToken)
        {
            return ResultType.Ok.GetValueCommitResults(await _dbContext.Set<Grade>().Where(a => a.IsEnabled && request.GradeIds.Contains(a.Id)).ProjectToType<GradeResponse>().ToListAsync(cancellationToken));
        }
    }
}
