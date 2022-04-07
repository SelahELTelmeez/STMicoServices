using CurriculumDomain.Features.Subjects.GetSubjectDetails.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetSubjectDetails.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjectDetails.CQRS.Query;
public class GetSubjectDetailsQueryHandler : IRequestHandler<GetSubjectDetailsQuery, CommitResult<SubjectDetailsResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetSubjectDetailsQueryHandler(CurriculumDbContext dbContext,
                                         IWebHostEnvironment configuration,
                                         IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<SubjectDetailsResponse>> Handle(GetSubjectDetailsQuery request, CancellationToken cancellationToken)
    {
        CurriculumEntites.Entities.Subjects.Subject? curriculumDetails = await _dbContext.Set<CurriculumEntites.Entities.Subjects.Subject>()
                                                                               .SingleOrDefaultAsync(a => a.Id.Equals(request.SubjectId), cancellationToken: cancellationToken);

        if (curriculumDetails == null)
        {
            return new CommitResult<SubjectDetailsResponse>
            {
                ErrorCode = "X0016",
                ErrorMessage = _resourceJsonManager["X0016"], //??? // Data of student Subject Details is not exist.
                ResultType = ResultType.NotFound,
            };
        }
        return new CommitResult<SubjectDetailsResponse>
        {
            ResultType = ResultType.Ok,
            Value = curriculumDetails.Adapt<SubjectDetailsResponse>(),
        };
    }
}