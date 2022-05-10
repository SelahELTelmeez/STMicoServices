using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityDomain.Features.GradesDropDown.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Grades;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.GradesDropDown.CQRS.Query
{
    public class GetGradeByIdQueryHandler : IRequestHandler<GetGradeByIdQuery, CommitResult<GradeResponse>>
    {
        private readonly STIdentityDbContext _dbContext;

        public GetGradeByIdQueryHandler(STIdentityDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommitResult<GradeResponse>> Handle(GetGradeByIdQuery request, CancellationToken cancellationToken)
        {
            return new CommitResult<GradeResponse>
            {
                ResultType = ResultType.Ok,
                Value = await _dbContext.Set<Grade>().Where(a => a.IsEnabled && a.Id == request.GradeId).ProjectToType<GradeResponse>().SingleOrDefaultAsync(cancellationToken)
            };
        }
    }
}
