using CurriculumDomain.Features.GetCurriculumUnit.CQRS.Query;
using CurriculumDomain.Features.GetStudentCurriculumDetails.DTO.Query;
using CurriculumEntites.Entities;
using DomainEntities = CurriculumEntites.Entities.Units;
using JsonLocalizer;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace CurriculumInfrastructure.Features.GetCurriculumUnit.CQRS.Query;
public class GetCurriculumUnitQueryHandler : IRequestHandler<GetCurriculumUnitQuery, CommitResult<List<CurriculumUnitResponseDTO>>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetCurriculumUnitQueryHandler(CurriculumDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;       
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult<List<CurriculumUnitResponseDTO>>> Handle(GetCurriculumUnitQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the Curriculum Id existance first, with the provided data.
        List<CurriculumUnitResponseDTO>? curriculumUnit = await _dbContext.Set<DomainEntities.Unit>()
                                                                             .Where(a => a.CurriculumId.Equals(request.CurriculumId))
                                                                             .Include(a => a.Lessons)
                                                                             .ProjectToType<CurriculumUnitResponseDTO>()
                                                                             .ToListAsync(cancellationToken);

        if (curriculumUnit == null)
        {
            return new CommitResult<List<CurriculumUnitResponseDTO>>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], // Data of student Curriculum Details is not exist.
                ResultType = ResultType.NotFound,
            };
        }

        return new CommitResult<List<CurriculumUnitResponseDTO>>
        {
            ResultType = ResultType.Ok,
            Value = curriculumUnit
        };
    }
}