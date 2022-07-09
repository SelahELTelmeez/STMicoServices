using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using SharedModule.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ChatInfrastructure.HttpClients;
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

    public async Task<ICommitResults<LimitedProfileResponse>?> GetLimitedProfilesAsync(IEnumerable<Guid> Identities, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("Identity/GetIdentityLimitedProfiles", Identities, cancellationToken);
        return await responseMessage.Content.ReadFromJsonAsync<CommitResults<LimitedProfileResponse>>(cancellationToken: cancellationToken);
    }
}