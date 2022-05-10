using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Grades;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using SharedModule.DTO;

namespace IdentityInfrastructure.Features.GradesDropDown.CQRS.Query;

public class GetGradesQueryHandler : IRequestHandler<GetGradesQuery, CommitResults<GradeResponse>>
{
    private readonly STIdentityDbContext _dbContext;

    public GetGradesQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResults<GradeResponse>> Handle(GetGradesQuery request, CancellationToken cancellationToken)
    {
        return new CommitResults<GradeResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Grade>().Where(a => a.IsEnabled).ProjectToType<GradeResponse>().ToListAsync()
        };
    }
}
