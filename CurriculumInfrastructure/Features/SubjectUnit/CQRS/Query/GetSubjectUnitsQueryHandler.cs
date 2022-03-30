using CurriculumDomain.Features.SubjectUnit.CQRS.Query;
using CurriculumDomain.Features.SubjectUnit.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using DomainEntities = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.SubjectUnit.CQRS.Query;
public class GetSubjectUnitsQueryHandler : IRequestHandler<GetSubjectUnitsQuery, CommitResult<List<SubjectUnitResponse>>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetSubjectUnitsQueryHandler(CurriculumDbContext dbContext,
                                         IWebHostEnvironment configuration,
                                         IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<List<SubjectUnitResponse>>> Handle(GetSubjectUnitsQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the Subject Id existance first, with the provided data.
        List<SubjectUnitResponse>? curriculumUnit = await _dbContext.Set<DomainEntities.Unit>()
                                                                             .Where(a => a.SubjectId.Equals(request.CurriculumId) && a.IsShow == true)
                                                                             .OrderBy(a => a.Sort)
                                                                             .Include(a => a.Lessons)
                                                                             .ProjectToType<SubjectUnitResponse>()
                                                                             .ToListAsync(cancellationToken);

        if (curriculumUnit == null)
        {
            return new CommitResult<List<SubjectUnitResponse>>
            {
                ErrorCode = "X0016",
                ErrorMessage = _resourceJsonManager["X0016"], // Data of student Subject Details is not exist.
                ResultType = ResultType.NotFound,
            };
        }

        return new CommitResult<List<SubjectUnitResponse>>
        {
            ResultType = ResultType.Ok,
            Value = curriculumUnit
        };
    }
}