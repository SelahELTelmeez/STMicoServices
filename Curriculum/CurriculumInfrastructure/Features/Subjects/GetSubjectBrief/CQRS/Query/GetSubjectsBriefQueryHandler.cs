using CurriculumDomain.Features.Subjects.GetSubjectBrief.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjectBrief.CQRS.Query;
public class GetSubjectBriefQueryHandler : IRequestHandler<GetSubjectBriefQuery, CommitResult<SubjectBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IDistributedCache _cache;

    public GetSubjectBriefQueryHandler(CurriculumDbContext dbContext,
                                         IWebHostEnvironment configuration,
                                         IHttpContextAccessor httpContextAccessor,
                                         IDistributedCache cache)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _cache = cache;
    }

    public async Task<CommitResult<SubjectBriefResponse>> Handle(GetSubjectBriefQuery request, CancellationToken cancellationToken)
    {
        SubjectBriefResponse? cachedSubjectBriefResponse = await _cache.GetFromCacheAsync<string, SubjectBriefResponse>(request.SubjectId, "Curriculum-GetSubjectBrief", cancellationToken);

        if (cachedSubjectBriefResponse == null)
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

            cachedSubjectBriefResponse = subject?.Adapt<SubjectBriefResponse>();

            await _cache.SaveToCacheAsync(request.SubjectId, cachedSubjectBriefResponse, "Curriculum-GetSubjectBrief", cancellationToken);
        }


        return new CommitResult<SubjectBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedSubjectBriefResponse
        };
    }
}