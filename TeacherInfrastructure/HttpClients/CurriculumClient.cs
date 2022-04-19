using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TeacherDomain.Features.Quiz.DTO;
using TeacherDomain.Features.TeacherSubject.DTO.Query;

namespace TeacherInfrastructure.HttpClients;
public class CurriculumClient
{
    private readonly HttpClient _httpClient;
    public CurriculumClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["CurriculumClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }

    public async Task<CommitResults<TeacherSubjectReponse>?> GetTeacherSubjectsDetailsAsync(IEnumerable<string> subjectIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/Curriculum/GetTeacjerSubjects", subjectIds, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResults<TeacherSubjectReponse>>();
    }

    public async Task<CommitResult<bool>?> VerifySubjectGradeMatchingAsync(string subjectId, int GradeId, CancellationToken cancellationToken)
        => await _httpClient.GetFromJsonAsync<CommitResult<bool>>($"/Curriculum/VerifySubjectGradeMatching?SubjectId={subjectId}&GradeId={GradeId}", cancellationToken);


    public async Task<CommitResult<int>?> CreateQuizeAsync(int clipId, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/Curriculum/CreateQuize", clipId, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult<int>>();
    }

    public async Task<CommitResult?> SubmitQuizeAsync(UserQuizAnswersRequest answersRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/Curriculum/SubmitQuize", answersRequest, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult>();
    }
}