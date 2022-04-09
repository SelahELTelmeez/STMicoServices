using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumInfrastructure.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CurriculumInfrastructure.HttpClients;

public class TrackerClient
{
    private readonly HttpClient _httpClient;
    public TrackerClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["TrackerClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResults<ClipActivityResponse>> GetClipActivitiesAsync(IEnumerable<int> ClipIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("/StudentActivityTracker/GetClipActivities", ClipIds, cancellationToken);
        return await responseMessage.Content.ReadFromJsonAsync<CommitResults<ClipActivityResponse>>(cancellationToken: cancellationToken);
    }
}
