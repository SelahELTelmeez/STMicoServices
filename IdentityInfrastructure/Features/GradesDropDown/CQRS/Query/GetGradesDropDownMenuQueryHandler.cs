using IdentityDomain.Features.GradesDropDown.CQRS.Query;
using IdentityDomain.Features.GradesDropDown.DTO;
using IdentityEntities.Entities;
using IdentityEntities.Entities.Grades;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace IdentityInfrastructure.Features.GradesDropDown.CQRS.Query;

public class GetGradesDropDownMenuQueryHandler : IRequestHandler<GetGradesDropDownMenuQuery, CommitResult<List<GradeDropDownMenuItemDTO>>>
{
    private readonly STIdentityDbContext _dbContext;

    public GetGradesDropDownMenuQueryHandler(STIdentityDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResult<List<GradeDropDownMenuItemDTO>>> Handle(GetGradesDropDownMenuQuery request, CancellationToken cancellationToken)
    {
        return new CommitResult<List<GradeDropDownMenuItemDTO>>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Grade>().Where(a => a.IsEnabled).ProjectToType<GradeDropDownMenuItemDTO>().ToListAsync()
        };
    }
}
