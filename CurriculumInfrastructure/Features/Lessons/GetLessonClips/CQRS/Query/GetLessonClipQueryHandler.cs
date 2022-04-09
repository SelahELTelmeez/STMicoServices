﻿using CurriculumDomain.Features.Lessons.GetLessonClips.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.Utilities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DomainEntities = CurriculumEntites.Entities.Clips;

namespace CurriculumInfrastructure.Features.Lessons.GetLessonClips.CQRS.Query;
public class GetLessonClipQueryHandler : IRequestHandler<GetLessonClipQuery, CommitResult<LessonClipResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly HttpClient _TrackerClient;

    public GetLessonClipQueryHandler(CurriculumDbContext dbContext,
                                    IHttpClientFactory factory,
                                    IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _TrackerClient = factory.CreateClient("TrackerClient");
        _TrackerClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _TrackerClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
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