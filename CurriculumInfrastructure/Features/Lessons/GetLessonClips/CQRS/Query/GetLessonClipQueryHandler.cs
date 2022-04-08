using CurriculumDomain.Features.Lessons.GetLessonClips.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DomainEntities = CurriculumEntites.Entities.Clips;

namespace CurriculumInfrastructure.Features.Lessons.GetLessonClips.CQRS.Query;
public class GetLessonClipQueryHandler : IRequestHandler<GetLessonClipQuery, CommitResult<LessonClipResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly HttpClient _TrackerClient;

    public GetLessonClipQueryHandler(CurriculumDbContext dbContext,
                                    IHttpClientFactory factory,
                                    IWebHostEnvironment configuration,
                                    IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _TrackerClient = factory.CreateClient("TrackerClient");
        _TrackerClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _TrackerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());

        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
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
            HttpResponseMessage responseMessage = await _TrackerClient.PostAsJsonAsync("/StudentActivityTracker/GetClipActivities", clips.Select(a => a.Id), cancellationToken);

            CommitResults<ClipActivityResponse>? ClipActivityResponses = await responseMessage.Content.ReadFromJsonAsync<CommitResults<ClipActivityResponse>>(cancellationToken: cancellationToken);

            IEnumerable<ClipResponse> Mapping()
            {
                foreach (DomainEntities.Clip clip in clips)
                {
                    ClipResponse clipResponse = clip.Adapt<ClipResponse>();
                    ClipActivityResponse? clipActivityResponse = ClipActivityResponses?.Value?.SingleOrDefault(a => a.ClipId.Equals(clip.Id));
                    if (clipActivityResponse != null)
                    {
                        clipResponse.ActivityId = clipActivityResponse.ActivityId;
                        clipResponse.StudentScore = clipActivityResponse.StudentScore;
                        clipResponse.GameObjectCode = clipActivityResponse.GameObjectCode;
                        clipResponse.GameObjectLearningDurationInSec = clipActivityResponse.GameObjectLearningDurationInSec;
                        clipResponse.GameObjectProgress = clipActivityResponse.GameObjectProgress;
                    }
                    yield return clipResponse;
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