using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransactionDomain.Features.Activities.DTO.Command;
using TransactionDomain.Features.IdentityScores.IdentityClipScore.DTO;
using TransactionDomain.Features.IdentityScores.IdentitySubjectScore.DTO;
using TransactionDomain.Features.Parent.DTO;
using TransactionDomain.Features.Tracker.DTO.Query;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.HttpClients;

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

    public async Task<CommitResults<ClipBriefResponse>?> GetClipsBriefAsync(int lessonId, CancellationToken cancellationToken)
    => await _httpClient.GetFromJsonAsync<CommitResults<ClipBriefResponse>>($"/Curriculum/GetClipsBrief?LessonId={lessonId}", cancellationToken);

    public async Task<CommitResult<SubjectBriefResponse>?> GetSubjectBriefAsync(string subjectId, CancellationToken cancellationToken)
    => await _httpClient.GetFromJsonAsync<CommitResult<SubjectBriefResponse>>($"/Curriculum/GetSubjectBrief?SubjectId={subjectId}", cancellationToken);

    public async Task<CommitResults<LessonBriefResponse>?> GetLessonsBriefBySubjectAsync(string subjectId, CancellationToken cancellationToken)
    => await _httpClient.GetFromJsonAsync<CommitResults<LessonBriefResponse>>($"/Curriculum/GetLessonsBriefBySubject?SubjectId={subjectId}", cancellationToken);
    public async Task<CommitResult<DetailedProgressResponse>?> GetSubjectDetailedProgressAsync(string subjectId, CancellationToken cancellationToken)
             => await _httpClient.GetFromJsonAsync<CommitResult<DetailedProgressResponse>>($"/Curriculum/GetSubjectDetailedProgressQuery?SubjectId={subjectId}", cancellationToken);

    public async Task<CommitResults<LessonBriefResponse>?> GetLessonsBriefAsync(IEnumerable<int> lessonIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponse = await _httpClient.PostAsJsonAsync($"/Curriculum/GetLessonsBrief", lessonIds, cancellationToken);

        if (httpResponse.IsSuccessStatusCode)
            return await httpResponse.Content.ReadFromJsonAsync<CommitResults<LessonBriefResponse>>(cancellationToken: cancellationToken);
        else
            return new CommitResults<LessonBriefResponse>
            {
                ResultType = ResultType.Invalid
            };
    }

    public async Task<CommitResult<bool>?> VerifySubjectGradeMatchingAsync(string subjectId, int GradeId, CancellationToken cancellationToken)
    => await _httpClient.GetFromJsonAsync<CommitResult<bool>>($"/Curriculum/VerifySubjectGradeMatching?SubjectId={subjectId}&GradeId={GradeId}", cancellationToken);

    public async Task<CommitResults<StudentSubjectResponse>?> GetStudentSubjectsAsync(int GradeId, CancellationToken cancellationToken)
   => await _httpClient.GetFromJsonAsync<CommitResults<StudentSubjectResponse>>($"/Curriculum/GetStudentSubjects?GradeId={GradeId}", cancellationToken);

}