using CurriculumDomain.Features.Lessons.GetLessonsBrief.CQRS.Query;
using CurriculumEntites.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;
using DomainEntities = CurriculumEntites.Entities.Lessons;
namespace CurriculumInfrastructure.Features.Lessons.GetLessonsBrief.CQRS.Query;

public class GetLessonsBriefQueryHandler : IRequestHandler<GetLessonsBriefQuery, CommitResults<LessonBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public GetLessonsBriefQueryHandler(CurriculumDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }
    public async Task<CommitResults<LessonBriefResponse>> Handle(GetLessonsBriefQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<LessonBriefResponse>? cachedLessonBreifResponses = await _cache.GetFromCacheAsync<string, IEnumerable<LessonBriefResponse>>(string.Join(',', request.LessonIds), "Curriculum-GetLessonsBrief", cancellationToken);

        if (cachedLessonBreifResponses == null)
        {
            cachedLessonBreifResponses = await _dbContext.Set<DomainEntities.Lesson>().Where(a => request.LessonIds.Contains(a.Id)).ProjectToType<LessonBriefResponse>().ToListAsync(cancellationToken);

            await _cache.SaveToCacheAsync(string.Join(',', request.LessonIds), cachedLessonBreifResponses, "Curriculum-GetLessonsBrief", cancellationToken);
        }

        return new CommitResults<LessonBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedLessonBreifResponses
        };
    }

}

