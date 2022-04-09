using CurriculumDomain.Features.Subjects.GetStudentSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetStudentSubjects.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.Utilities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using CurriculumEntities = CurriculumEntites.Entities.Subjects;
namespace CurriculumInfrastructure.Features.Subjects.GetStudentSubjects.CQRS.Query;
public class GetStudentSubjectsQueryHandler : IRequestHandler<GetStudentSubjectsQuery, CommitResults<IdnentitySubjectResponse>>
{
    private readonly HttpClient _IdentityClient;
    private readonly CurriculumDbContext _dbContext;

    public GetStudentSubjectsQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
        _IdentityClient = factory.CreateClient("IdentityClient");
        _IdentityClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _IdentityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }

    public async Task<CommitResults<IdnentitySubjectResponse>> Handle(GetStudentSubjectsQuery request, CancellationToken cancellationToken)
    {

        // Calling Identity Micro-service to get the current grade of the user.
        CommitResult<int>? commitResult = await _IdentityClient.GetFromJsonAsync<CommitResult<int>>("/Provider/GetIdentityGrade", cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return commitResult.Adapt<CommitResults<IdnentitySubjectResponse>>();
        }

        return new CommitResults<IdnentitySubjectResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<CurriculumEntities.Subject>().Where(a => a.Grade == commitResult.Value && a.IsAppShow == true).ProjectToType<IdnentitySubjectResponse>().ToListAsync(cancellationToken)
        };
    }
}
