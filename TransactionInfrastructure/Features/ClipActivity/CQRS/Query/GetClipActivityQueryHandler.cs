using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
using TransactionDomain.Features.ClipActivity.CQRS.Query;
using TransactionDomain.Features.ClipActivity.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.Trackers;

namespace TransactionInfrastructure.Features.ClipActivity.CQRS.Query;
public class GetClipActivityQueryHandler : IRequestHandler<GetClipActivityQuery, CommitResult<IEnumerable<ClipActivityResponse>>>
{
    private readonly StudentTrackerDbContext _dbContext;
    private readonly HttpClient _IdentityClient;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public GetClipActivityQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, StudentTrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _IdentityClient = factory.CreateClient("IdentityClient");
        _IdentityClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _IdentityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }
    public async Task<CommitResult<IEnumerable<ClipActivityResponse>>> Handle(GetClipActivityQuery request, CancellationToken cancellationToken)
    {
        List<DomainEntities.StudentActivityTracker> ClipActivities = await _dbContext.Set<DomainEntities.StudentActivityTracker>()
                                                                      .Where(a => request.ClipIds.Contains(a.ClipId) && a.StudentId.Equals(_httpContextAccessor.GetIdentityUserId()))
                                                                      .ToListAsync(cancellationToken);
        
        return new CommitResult<IEnumerable<ClipActivityResponse>>
        {
            ResultType = ResultType.Ok,
            Value = ClipActivities.Adapt<IEnumerable<ClipActivityResponse>>()
        };
    }
}
