using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using TransactionDomain.Features.Invitations.CQRS.DTO.Query;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.HttpClients
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

        public async Task<CommitResults<IdentityUserInvitationResponse>?> GetIdentityUserInvitationsAsync(IEnumerable<Guid> InviterIds, CancellationToken cancellationToken)
        {
            HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("/Identity/GetIdentityUserInvitations", InviterIds, cancellationToken);

            return await responseMessage.Content.ReadFromJsonAsync<CommitResults<IdentityUserInvitationResponse>>(cancellationToken: cancellationToken);
        }

    }
}
