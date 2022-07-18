using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TeacherInfrastructure.HttpClients;
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


    public async Task<ICommitResult<LimitedProfileResponse>?> GetIdentityLimitedProfileAsync(string IdentityId, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResult<LimitedProfileResponse>>($"Identity/GetIdentityLimitedProfile?IdentityId={IdentityId}", cancellationToken);
    }

    public async Task<ICommitResults<LimitedProfileResponse>?> GetIdentityLimitedProfilesAsync(IEnumerable<string> IdentityIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("Identity/GetIdentityLimitedProfiles", IdentityIds, cancellationToken);

        return await responseMessage.Content.ReadFromJsonAsync<CommitResults<LimitedProfileResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<ICommitResults<LimitedProfileResponse>?> GetTeacherLimitedProfilesByNameOrMobileNumberAsync(string NameOrMobile, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResults<LimitedProfileResponse>>($"Identity/GetTeacherLimitedProfilesByNameOrMobile?NameOrMobile={NameOrMobile}", cancellationToken);
    }
}