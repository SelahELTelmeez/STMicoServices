using Flaminco.CommitResult;
using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Grades;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.GradesDropDown.CQRS.Query
{
    public class GetGradeByIdQueryHandler : IRequestHandler<GetGradeByIdQuery, ICommitResult<GradeResponse>>
    {
        private readonly STIdentityDbContext _dbContext;

        public GetGradeByIdQueryHandler(STIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ICommitResult<GradeResponse>> Handle(GetGradeByIdQuery request, CancellationToken cancellationToken)
        {
            return ResultType.Ok.GetValueCommitResult(await _dbContext.Set<Grade>().Where(a => a.IsEnabled && a.Id == request.GradeId).ProjectToType<GradeResponse>().FirstOrDefaultAsync(cancellationToken));
        }
    }
}
