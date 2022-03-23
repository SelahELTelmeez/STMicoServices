using CurriculumDomain.Features.GetCurriculumUnit.CQRS.Query;
using CurriculumDomain.Features.GetStudentCurriculumDetails.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using DomainEntities = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.GetCurriculumUnit.CQRS.Query;
public class GetCurriculumUnitQueryHandler : IRequestHandler<GetCurriculumUnitQuery, CommitResult<List<CurriculumUnitResponseDTO>>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetCurriculumUnitQueryHandler(CurriculumDbContext dbContext,
                                         IWebHostEnvironment configuration,
                                         IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
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
                ErrorCode = "X0016",
                ErrorMessage = _resourceJsonManager["X0016"], // Data of student Curriculum Details is not exist.
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