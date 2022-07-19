using Flaminco.CommitResult;
using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using System.Net.Http.Json;

namespace PaymentInfrastructure.HttpClients
{
    public class IdentityClient
    {
        private readonly HttpClient _httpClient;
        public IdentityClient(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration["IdentityClient:baseUrl"]);
        }

        public async Task<ICommitResult<LimitedProfileResponse>?> GetIdentityLimitedProfileAsync(string? IdentityId, CancellationToken cancellationToken)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<CommitResult<LimitedProfileResponse>>($"Identity/GetIdentityLimitedProfile?IdentityId={IdentityId}", cancellationToken);

            }
            catch (HttpRequestException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
