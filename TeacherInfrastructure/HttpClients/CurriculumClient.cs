using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TeacherDomain.Features.Quiz.DTO.Command;

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

    public async Task<ICommitResults<SubjectResponse>?> GetSubjectsDetailsAsync(IEnumerable<string> subjectIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Curriculum/GetSubjects", subjectIds, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResults<SubjectResponse>>();
    }

    public async Task<ICommitResults<SubjectBriefResponse>?> GetSubjectsBriefAsync(IEnumerable<string> SubjectIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Curriculum/GetSubjectsBrief", SubjectIds, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResults<SubjectBriefResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<ICommitResults<TeacherSubjectResponse>?> GetTeacherSubjectsDetailsAsync(IEnumerable<string> subjectIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Curriculum/GetTeacherSubjects", subjectIds, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResults<TeacherSubjectResponse>>();
    }

    public async Task<ICommitResult<bool>?> VerifySubjectGradeMatchingAsync(string subjectId, int GradeId, CancellationToken cancellationToken)
        => await _httpClient.GetFromJsonAsync<CommitResult<bool>>($"Curriculum/VerifySubjectGradeMatching?SubjectId={subjectId}&GradeId={GradeId}", cancellationToken);


    public async Task<ICommitResult<int>?> CreateQuizeAsync(int clipId, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Curriculum/CreateQuize", clipId, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult<int>>();
    }

    public async Task<ICommitResult?> SubmitQuizeAsync(UserQuizAnswersRequest answersRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Curriculum/SubmitQuiz", answersRequest, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult>();
    }
}