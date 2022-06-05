﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ResultHandler;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace IdentityInfrastructure.HttpClients;

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

    public async Task<CommitResult?> SetAsInActiveInvitationAsync(int invitationId, CancellationToken cancellationToken)
    {
        return await _httpClient.GetFromJsonAsync<CommitResult>($"Notifier/SetAsInActive?invitationId={invitationId}", cancellationToken);
    }
}