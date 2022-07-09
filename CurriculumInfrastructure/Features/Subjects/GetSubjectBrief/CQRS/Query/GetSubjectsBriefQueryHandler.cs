using CurriculumDomain.Features.Subjects.GetSubjectBrief.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjectBrief.CQRS.Query;
public class GetSubjectBriefQueryHandler : IRequestHandler<GetSubjectBriefQuery, CommitResult<SubjectBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetSubjectBriefQueryHandler(CurriculumDbContext dbContext,
                                         IWebHostEnvironment configuration,
                                         IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<SubjectBriefResponse>> Handle(GetSubjectBriefQuery request, CancellationToken cancellationToken)
    {
        Subject? subject = await _dbContext.Set<Subject>()
                                           .FirstOrDefaultAsync(a => a.Id.Equals(request.SubjectId), cancellationToken: cancellationToken);

        if (subject == null)
        {
            return new CommitResult<SubjectBriefResponse>
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "XCUR0004",
                ErrorMessage = _resourceJsonManager["XCUR0004"]
            };
        }

        return new CommitResult<SubjectBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = subject?.Adapt<SubjectBriefResponse>()
        };
    }
}