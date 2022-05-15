using CurriculumDomain.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CurriculumInfrastructure.HttpClients;


public class IdentityClient : IIdentityClient
{
    private readonly HttpClient _httpClient;
    public IdentityClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["IdentityClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }

    public async Task<CommitResult<int>?> GetStudentGradesAsync(Guid? StudentId, CancellationToken cancellationToken)
    {
        if (StudentId == null)
        {
            return await _httpClient.GetFromJsonAsync<CommitResult<int>>("Identity/GetIdentityGrade", cancellationToken);
        }
        else
        {
            return await _httpClient.GetFromJsonAsync<CommitResult<int>>($"Identity/GetIdentityGrade?IdentityId={StudentId}", cancellationToken);
        }
    }

    public async Task<CommitResults<GradeResponse>?> GetGradesDetailesAsync(IEnumerable<int> GradeIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("Lookup/GetGradeByIds", GradeIds, cancellationToken);
        return await responseMessage.Content.ReadFromJsonAsync<CommitResults<GradeResponse>>(cancellationToken: cancellationToken);
    }
}
