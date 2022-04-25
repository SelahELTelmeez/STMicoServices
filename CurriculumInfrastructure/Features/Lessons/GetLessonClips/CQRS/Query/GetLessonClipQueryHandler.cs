using CurriculumDomain.Features.Lessons.GetLessonClips.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.HttpClients;
using Mapster;
using Microsoft.EntityFrameworkCore;
using DomainEntities = CurriculumEntites.Entities.Clips;

namespace CurriculumInfrastructure.Features.Lessons.GetLessonClips.CQRS.Query;
public class GetLessonClipQueryHandler : IRequestHandler<GetLessonClipQuery, CommitResult<LessonClipResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly StudentClient _TrackerClient;

    public GetLessonClipQueryHandler(CurriculumDbContext dbContext,
                                    StudentClient trackerClient)
    {
        _dbContext = dbContext;
        _TrackerClient = trackerClient;
    }

    public async Task<CommitResult<LessonClipResponse>> Handle(GetLessonClipQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the Subject Id existance first, with the provided data.
        List<DomainEntities.Clip>? clips = await _dbContext.Set<DomainEntities.Clip>()
                                            .Where(a => a.LessonId.Equals(request.LessonId))
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
                    ClipActivityResponse? clipActivityResponse = ClipActivityResponses?.Value?.SingleOrDefault(a => a.ClipId.Equals(clip.Id));
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
            return new CommitResult<LessonClipResponse>
            {
                ResultType = ResultType.Ok,
                Value = new LessonClipResponse
                {
                    Clips = Mapping(),
                    Types = clips.Adapt<List<FilterTypesResponse>>().Distinct()
                }
            };
        }
        return new CommitResult<LessonClipResponse>
        {
            ResultType = ResultType.Ok,
            Value = default
        };

        //// Get List Of Clip Activity

    }
}