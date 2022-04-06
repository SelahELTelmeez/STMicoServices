using CurriculumDomain.Features.LessonClip.CQRS.Query;
using CurriculumDomain.Features.LessonClip.DTO.Query;
using CurriculumEntites.Entities;
using DomainEntities = CurriculumEntites.Entities.Clips;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Json;
using CurriculumInfrastructure.Features.LessonClip.DTO.Query;

namespace CurriculumInfrastructure.Features.LessonClip.CQRS.Query;
public class GetLessonClipQueryHandler : IRequestHandler<GetLessonClipQuery, CommitResult<LessonClipResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly HttpClient _ClipActivityClient;

    public GetLessonClipQueryHandler(CurriculumDbContext dbContext,
                                           IWebHostEnvironment configuration,
                                           IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
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

        if (clips == null)
        {
            return new CommitResult<LessonClipResponse>
            {
                ErrorCode = "X0015",
                ErrorMessage = _resourceJsonManager["X0015"], // Data of student Subject Details is not exist.
                ResultType = ResultType.NotFound,
            };
        }

        //// Get List Of Clip Activity
        HttpResponseMessage responseMessage = await _ClipActivityClient.PostAsJsonAsync("/StudentActivityTracker/GetClipActivities", clips.Select(a => a.Id), cancellationToken);

        CommitResult<List<ClipActivityResponse>>? ClipActivityResponses = await responseMessage.Content.ReadFromJsonAsync<CommitResult<List<ClipActivityResponse>>>(cancellationToken: cancellationToken);


        IEnumerable<ClipResponse> Mapping()
        {
            foreach (DomainEntities.Clip clip in clips)
            {
                ClipResponse clipResponse = clip.Adapt<ClipResponse>();
                ClipActivityResponse clipActivityResponse = ClipActivityResponses.Value.SingleOrDefault(a => a.ClipId.Equals(clip.Id));
                clipResponse.ActivityId = clipActivityResponse.ActivityId;
                clipResponse.StudentScore = clipActivityResponse.StudentScore;
                clipResponse.GameObjectCode = clipActivityResponse.GameObjectCode;
                clipResponse.GameObjectLearningDurationInSec = clipActivityResponse.GameObjectLearningDurationInSec;
                clipResponse.GameObjectProgress = clipActivityResponse.GameObjectProgress;
                yield return clipResponse;
            }
        }

        return new CommitResult<LessonClipResponse>
        {
            ResultType = ResultType.Ok,
            Value = new LessonClipResponse
            {
                Clips = Mapping().ToList(),
                Types = clips.Adapt<List<FilterTypesResponse>>().Distinct().ToList()
            }
        };
    }
}