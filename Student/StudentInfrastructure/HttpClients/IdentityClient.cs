using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using SharedModule.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace StudentInfrastructure.HttpClients
{
    public class IdentityClient
    {
        private readonly HttpClient _httpClient;
        public IdentityClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["IdentityClient:baseUrl"]);
            _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
        }

        public async Task<ICommitResult<int>?> GetStudentGradeAsync(string? StudentId, CancellationToken cancellationToken)
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

        public async Task<ICommitResult<LimitedProfileResponse>?> GetIdentityLimitedProfileAsync(string IdentityId, CancellationToken cancellationToken)
        {
            return await _httpClient.GetFromJsonAsync<CommitResult<LimitedProfileResponse>>($"Identity/GetIdentityLimitedProfile?IdentityId={IdentityId}", cancellationToken);
        }
    }
}
