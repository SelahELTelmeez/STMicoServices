﻿using Microsoft.Extensions.Configuration;
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
    public async Task<CommitResults<StudentQuizResultResponse>?> GetQuizzesResultAsync(Guid StudentId, IEnumerable<int> QuizIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Student/GetStudentQuizResults", new StudentQuizResultRequest { StudentId = StudentId, QuizIds = QuizIds}  , cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResults<StudentQuizResultResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<CommitResults<StudentQuizResultResponse>> GetQuizzesResultAsync(IEnumerable<Guid> StudentIds, IEnumerable<int> QuizIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Student/GetStudentsQuizResults", new StudentsQuizResultRequest { StudentIds = StudentIds, QuizIds = QuizIds }, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResults<StudentQuizResultResponse>>(cancellationToken: cancellationToken);
    }
    public async Task<CommitResult<StudentQuizResultResponse>?> GetQuizResultAsync(Guid StudentId, int QuizId, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResult<StudentQuizResultResponse>>($"Student/GetStudentQuizResult?StudentId={StudentId}&QuizId={QuizId}", cancellationToken: cancellationToken);
    }
}
