using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ParentDomain.Features.Shared.DTO;
using ResultHandler;
using SharedModule.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ParentInfrastructure.HttpClients;

public class NotifierClient
{
    private readonly HttpClient _httpClient;
    public NotifierClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["NotifierClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }

    public async Task<CommitResult?> SendNotificationAsync(NotificationRequest notificationRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/Notifier/Send", notificationRequest, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult>();
    }

    public async Task<CommitResult?> SendInvitationAsync(InvitationRequest invitationRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/Notifier/Send", invitationRequest, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult>();
    }

    public async Task<CommitResult?> SetAsInActiveInvitationAsync(int invitationId, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResult>($"/Notifier/SetAsInActive?invitationId={invitationId}", cancellationToken);
    }
}
