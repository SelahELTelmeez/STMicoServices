using CurriculumDomain.Features.IdnentitySubject.CQRS.Query;
using CurriculumDomain.Features.IdnentitySubject.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using CurriculumInfrastructure.Utilities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CurriculumInfrastructure.Features.IdnentitySubject.CQRS.Query;
public class GetIdnentitySubjectsQueryHandler : IRequestHandler<GetIdentitySubjectsQuery, CommitResult<List<IdnentitySubjectResponse>>>
{
    private readonly HttpClient _IdentityClient;
    private readonly CurriculumDbContext _dbContext;

    public GetIdnentitySubjectsQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
        _IdentityClient = factory.CreateClient("IdentityClient");
        _IdentityClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _IdentityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }

    public async Task<CommitResult<List<IdnentitySubjectResponse>>> Handle(GetIdentitySubjectsQuery request, CancellationToken cancellationToken)
    {

        // Calling Identity Micro-service to get the current grade of the user.
        CommitResult<int>? commitResult = await _IdentityClient.GetFromJsonAsync<CommitResult<int>>("/identity/GetIdentityGrade", cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return commitResult.Adapt<CommitResult<List<IdnentitySubjectResponse>>>();
        }

        return new CommitResult<List<IdnentitySubjectResponse>>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Subject>().Where(a => a.Grade == commitResult.Value && a.IsAppShow == true).ProjectToType<IdnentitySubjectResponse>().ToListAsync(cancellationToken)
        };
    }
}
