using Flaminco.CommitResult;
using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Grades;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.GradesDropDown.CQRS.Query;

public class GetGradesQueryHandler : IRequestHandler<GetGradesQuery, ICommitResults<GradeResponse>>
{
    private readonly STIdentityDbContext _dbContext;

    public GetGradesQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<ICommitResults<GradeResponse>> Handle(GetGradesQuery request, CancellationToken cancellationToken)
    {
        return ResultType.Ok.GetValueCommitResults(await _dbContext.Set<Grade>().Where(a => a.IsEnabled).ProjectToType<GradeResponse>().ToListAsync());
    }
}
