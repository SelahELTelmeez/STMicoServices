using CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
using CurriculumDomain.Features.Quizzes.DTO.Command;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CurriculumInfrastructure.HttpClients;

public class StudentClient
{
    private readonly HttpClient _httpClient;
    public StudentClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["StudentClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResults<ClipActivityResponse>?> GetClipActivitiesAsync(IEnumerable<int> ClipIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("/ActivityTracker/GetClipActivities", ClipIds, cancellationToken);
        return await responseMessage.Content.ReadFromJsonAsync<CommitResults<ClipActivityResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<CommitResult> SubmitStudentQuizAnswerAsync(UpdateStudentQuizRequest request, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("/ActivityTracker/SubmitStudentQuizAnswer", request, cancellationToken);
        return new CommitResult
        {
            ResultType = responseMessage.IsSuccessStatusCode ? ResultType.Ok : ResultType.Invalid
        };
    }
}
