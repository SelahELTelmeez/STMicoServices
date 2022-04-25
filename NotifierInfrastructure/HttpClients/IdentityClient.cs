using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NotifierDomain.Features.Invitations.CQRS.DTO.Query;
using NotifierDomain.Features.Shared.DTO;
using SharedModule.Extensions;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace NotifierInfrastructure.HttpClients;
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

    public async Task<CommitResults<IdentityUserNotificationResponse>?> GetIdentityUserNotificationsAsync(IEnumerable<Guid> NotifierIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("/Identity/GetIdentityUserNotifications", NotifierIds, cancellationToken);

        return await responseMessage.Content.ReadFromJsonAsync<CommitResults<IdentityUserNotificationResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<CommitResult<LimitedProfileResponse>?> GetIdentityLimitedProfileAsync(Guid IdentityId, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResult<LimitedProfileResponse>>($"/Identity/GetIdentityLimitedProfile?IdentityId={IdentityId}", cancellationToken);

    }

    public async Task<CommitResults<LimitedProfileResponse>?> GetIdentityLimitedProfilesAsync(IEnumerable<Guid> IdentityIds, CancellationToken cancellationToken)
    {
        HttpResponseMessage responseMessage = await _httpClient.PostAsJsonAsync("/Identity/GetIdentityLimitedProfiles", IdentityIds, cancellationToken);

        return await responseMessage.Content.ReadFromJsonAsync<CommitResults<LimitedProfileResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<CommitResults<LimitedProfileResponse>?> GetTeacherLimitedProfilesByNameOrMobileNumberAsync(string NameOrMobile, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResults<LimitedProfileResponse>>($"/Identity/GetTeacherLimitedProfilesByNameOrMobile?NameOrMobile={NameOrMobile}", cancellationToken);

    }
}