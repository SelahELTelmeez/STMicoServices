using CurriculumDomain.Features.Lessons.GetClipsBrief.CQRS.Query;
using CurriculumEntites.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;
using DomainEntitiesClips = CurriculumEntites.Entities.Clips;

namespace CurriculumInfrastructure.Features.Lessons.GetClipsBrief.CQRS.Query;
public class GetClipsBriefByLessonIdQueryHandler : IRequestHandler<GetClipsBriefByLessonIdQuery, CommitResults<ClipBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly IDistributedCache _cache;
    public GetClipsBriefByLessonIdQueryHandler(CurriculumDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<CommitResults<ClipBriefResponse>> Handle(GetClipsBriefByLessonIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ClipBriefResponse>? cachedClipBriefResponses = await _cache.GetFromCacheAsync<int, IEnumerable<ClipBriefResponse>>(request.LessonId, "Curriculum-GetClipsBrief", cancellationToken);

        if (cachedClipBriefResponses == null)
        {
            cachedClipBriefResponses = await _dbContext.Set<DomainEntitiesClips.Clip>()
                                                       .Where(a => a.LessonId.Equals(request.LessonId))
                                                       .Select(a => new ClipBriefResponse
                                                       {
                                                           Id = a.Id,
                                                           Name = a.Title,
                                                           Ponits = a.Points,
                                                       }).ToListAsync(cancellationToken);

            await _cache.SaveToCacheAsync(request.LessonId, cachedClipBriefResponses, "Curriculum-GetClipsBrief", cancellationToken);
        }

        return new CommitResults<ClipBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedClipBriefResponses
        };
    }
}