using CurriculumDomain.Features.Subjects.GetSubjectBrief.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjectBrief.CQRS.Query;
public class GetSubjectsBriefQueryHandler : IRequestHandler<GetSubjectsBriefQuery, CommitResults<SubjectBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public GetSubjectsBriefQueryHandler(CurriculumDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<CommitResults<SubjectBriefResponse>> Handle(GetSubjectsBriefQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<SubjectBriefResponse>? cachedSubjectBriefResponses = await _cache.GetFromCacheAsync<string, IEnumerable<SubjectBriefResponse>>(string.Join(',', request.SubjectIds), "Curriculum-GetSubjectsBrief", cancellationToken);

        if (cachedSubjectBriefResponses == null)
        {
            cachedSubjectBriefResponses = await _dbContext.Set<Subject>().Where(a => request.SubjectIds.Contains(a.Id)).ProjectToType<SubjectBriefResponse>().ToListAsync(cancellationToken);

            await _cache.SaveToCacheAsync(string.Join(',', request.SubjectIds), cachedSubjectBriefResponses, "Curriculum-GetSubjectsBrief", cancellationToken);
        }
        return new CommitResults<SubjectBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedSubjectBriefResponses
        };
    }
}