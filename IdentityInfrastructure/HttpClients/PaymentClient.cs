using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ResultHandler;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IdentityInfrastructure.HttpClients;

public class PaymentClient
{
    private readonly HttpClient _httpClient;
    public PaymentClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["PaymentClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<bool>?> ValidateCurrentUserPaymentStatusAsync(Guid? UserId, string? Token, CancellationToken cancellationToken)
    {
        if (Token != null)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
        }
        return await _httpClient.GetFromJsonAsync<CommitResult<bool>>($"Payment/ValidateCurrentUserPaymentStatus?UserId={UserId}", cancellationToken);
    }
}
