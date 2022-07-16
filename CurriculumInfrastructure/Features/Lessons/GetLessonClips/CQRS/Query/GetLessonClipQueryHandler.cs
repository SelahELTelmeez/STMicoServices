using CurriculumDomain.Features.Lessons.GetLessonClips.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.HttpClients;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;
using DomainEntities = CurriculumEntites.Entities.Clips;

namespace CurriculumInfrastructure.Features.Lessons.GetLessonClips.CQRS.Query;
public class GetLessonClipQueryHandler : IRequestHandler<GetLessonClipQuery, CommitResult<LessonClipResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly StudentClient _TrackerClient;
    private readonly IDistributedCache _cache;

    public GetLessonClipQueryHandler(CurriculumDbContext dbContext,
                                    StudentClient trackerClient,
                                    IDistributedCache cache)
    {
        _dbContext = dbContext;
        _TrackerClient = trackerClient;
        _cache = cache;
    }

    public async Task<CommitResult<LessonClipResponse>> Handle(GetLessonClipQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the Subject Id existance first, with the provided data.

        LessonClipResponse? cachedLessonClipResponse = await _cache.GetFromCacheAsync<int, LessonClipResponse>(request.LessonId, "Curriculum-GetLessonClip", cancellationToken);

        if (cachedLessonClipResponse == null)
        {
            List<DomainEntities.Clip>? clips = await _dbContext.Set<DomainEntities.Clip>()
                                         .Where(a => a.LessonId.Equals(request.LessonId) && a.Usability >= 2)
                                         .OrderBy(a => a.Sort)
                                         .Include(a => a.LessonFK)
                                         .ThenInclude(a => a.UnitFK)
                                         .ThenInclude(a => a.SubjectFK)
                                         .ToListAsync(cancellationToken);

            if (clips.Any())
            {
                CommitResults<ClipActivityResponse>? ClipActivityResponses = await _TrackerClient.GetClipActivitiesAsync(clips.Select(a => a.Id), cancellationToken);

                IEnumerable<ClipResponse> Mapping()
                {
                    foreach (DomainEntities.Clip clip in clips)
                    {
                        ClipActivityResponse? clipActivityResponse = ClipActivityResponses?.Value?.FirstOrDefault(a => a.ClipId.Equals(clip.Id));
                        if (clipActivityResponse != null)
                        {
                            yield return (clip, clipActivityResponse).Adapt<ClipResponse>();
                        }
                        else
                        {
                            yield return clip.Adapt<ClipResponse>();
                        }
                    }
                }

                cachedLessonClipResponse = new LessonClipResponse
                {
                    Clips = Mapping(),
                    Types = clips.Adapt<List<FilterTypesResponse>>().Distinct()
                };

                await _cache.SaveToCacheAsync(request.LessonId, cachedLessonClipResponse, "Curriculum-GetLessonClip", cancellationToken);
            }
            else
            {
                return new CommitResult<LessonClipResponse>
                {
                    ResultType = ResultType.Empty,
                    Value = default,
                };
            }
        }
        return new CommitResult<LessonClipResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedLessonClipResponse
        };



    }
}