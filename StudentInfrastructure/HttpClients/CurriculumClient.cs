using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using SharedModule.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StudentInfrastructure.HttpClients;

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

    public async Task<ICommitResults<ClipBriefResponse>?> GetClipsBriefAsync(int lessonId, CancellationToken cancellationToken)
      => await _httpClient.GetFromJsonAsync<CommitResults<ClipBriefResponse>>($"Curriculum/GetClipsBrief?LessonId={lessonId}", cancellationToken);

    public async Task<ICommitResult<SubjectBriefResponse>?> GetSubjectBriefAsync(string subjectId, CancellationToken cancellationToken)
      => await _httpClient.GetFromJsonAsync<CommitResult<SubjectBriefResponse>>($"Curriculum/GetSubjectBrief?SubjectId={subjectId}", cancellationToken);

    public async Task<ICommitResults<SubjectBriefResponse>?> GetSubjectsBriefByTermAsync(int Grade, int TermId, CancellationToken cancellationToken)
        => await _httpClient.GetFromJsonAsync<CommitResults<SubjectBriefResponse>>($"Curriculum/GetSubjectsBriefByTerm?Grade={Grade}&Term={TermId}", cancellationToken);


    public async Task<ICommitResults<LessonBriefResponse>?> GetLessonsBriefBySubjectAsync(string subjectId, CancellationToken cancellationToken)
    => await _httpClient.GetFromJsonAsync<CommitResults<LessonBriefResponse>>($"Curriculum/GetLessonsBriefBySubject?SubjectId={subjectId}", cancellationToken);

    public async Task<ICommitResult<DetailedProgressResponse>?> GetSubjectDetailedProgressAsync(string subjectId, CancellationToken cancellationToken)
             => await _httpClient.GetFromJsonAsync<CommitResult<DetailedProgressResponse>>($"Curriculum/GetSubjectDetailedProgress?SubjectId={subjectId}", cancellationToken);

    public async Task<ICommitResults<SubjectBriefProgressResponse>?> SubjectsBriefProgressAsync(int Term, Guid? StudentId, CancellationToken cancellationToken)
         => await _httpClient.GetFromJsonAsync<CommitResults<SubjectBriefProgressResponse>>($"Curriculum/GetSubjectsBriefProgress?Term={Term}&StudentId={StudentId}", cancellationToken);


    public async Task<ICommitResults<LessonBriefResponse>?> GetLessonsBriefAsync(IEnumerable<int> lessonIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync($"Curriculum/GetLessonsBrief", lessonIds, cancellationToken);
        return await httpResponse.Content.ReadFromJsonAsync<CommitResults<LessonBriefResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<ICommitResults<SubjectDetailedResponse>?> GetSubjectsDetailedAsync(IEnumerable<string> subjectIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync($"Curriculum/GetSubjectsDetailed", subjectIds, cancellationToken);
        return await httpResponse.Content.ReadFromJsonAsync<CommitResults<SubjectDetailedResponse>>(cancellationToken: cancellationToken);
    }

}