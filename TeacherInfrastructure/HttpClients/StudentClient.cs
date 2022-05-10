using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TeacherDomain.Features.Tracker.DTO.Query;

namespace TeacherInfrastructure.HttpClients;

public class StudentClient
{
    private readonly HttpClient _httpClient;
    public StudentClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["CurriculumClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResults<StudentQuizResultResponse>?> GetSubjectsDetailsAsync(StudentQuizResultRequest studentQuizResult, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Student/GetStudentQuizResults", studentQuizResult, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResults<StudentQuizResultResponse>>(cancellationToken: cancellationToken);
    }
}
