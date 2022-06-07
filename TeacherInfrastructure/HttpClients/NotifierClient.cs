using Microsoft.Extensions.Configuration;
using SharedModule.DTO;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace TeacherInfrastructure.HttpClients;

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

    public async Task<ICommitResult?> SendNotificationAsync(NotificationRequest notificationRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Notifier/SendNotification", notificationRequest, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult>();
    }

    public async Task<ICommitResult?> SendInvitationAsync(InvitationRequest invitationRequest, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Notifier/SendInvitation", invitationRequest, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResult>();
    }
    public async Task<ICommitResults<ClassStatusResponse>?> GetClassesStatusAsync(IEnumerable<int> classIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage httpResponseMessage = await _httpClient.PostAsJsonAsync($"Notifier/GetClassesCurrentStatus", classIds, cancellationToken);
        return await httpResponseMessage.Content.ReadFromJsonAsync<CommitResults<ClassStatusResponse>>();
    }

    public async Task<ICommitResult?> SetAsInActiveInvitationAsync(int invitationId, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResult>($"Notifier/SetAsInActive?invitationId={invitationId}", cancellationToken);
    }
}
