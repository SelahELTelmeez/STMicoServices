using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using StudentDomain.Features.Shared.DTO;
using StudentInfrastructure.Utilities;

namespace StudentInfrastructure.HttpClients;
public class NotifierClient
{
    private readonly HttpClient _httpClient;
    public NotifierClient(HttpClient httpClient, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(configuration["CurriculumClient:baseUrl"]);
        _httpClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }

    public async Task<CommitResult?> SendNotificationAsync(NotificationRequest notificationRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/Notification/Send", notificationRequest, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult>();
    }

    public async Task<CommitResult?> SendInvitationAsync(InvitationRequest invitationRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"/Invitation/Send", invitationRequest, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult>();
    }

    public async Task<CommitResult?> SetAsInActiveInvitationAsync(int invitationId, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResult>($"/Invitation/SetAsInActive?invitationId={invitationId}", cancellationToken);
    }
}
