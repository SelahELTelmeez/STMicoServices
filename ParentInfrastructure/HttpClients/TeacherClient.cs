using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ParentDomain.Features.Parent.DTO.Query;
using ResultHandler;
using SharedModule.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ParentInfrastructure.HttpClients;
public class TeacherClient
{
    private readonly HttpClient _httpClient;
    public TeacherClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["TeacherClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }

    public async Task<CommitResults<ClassesEntrolledByStudentResponse>?> GetClassesEntrolledByStudentAsync(Guid studentId, CancellationToken cancellationToken)
    {
        CommitResults<ClassesEntrolledByStudentResponse>? test = await _httpClient.GetFromJsonAsync<CommitResults<ClassesEntrolledByStudentResponse>>($"/Teacher/GetClassesEntrolledByStudent?StudentId={studentId}", cancellationToken);
        return test;
    }
    //    => await _httpClient.GetFromJsonAsync<CommitResults<ParentHomeDataResponse>>($"/Teacher/GetClassesEntrolledByStudent?StudentId={studentId}", cancellationToken);
}
