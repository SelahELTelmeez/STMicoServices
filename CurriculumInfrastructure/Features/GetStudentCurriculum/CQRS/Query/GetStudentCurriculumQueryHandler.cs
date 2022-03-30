using CurriculumDomain.Features.GetStudentCurriculum.CQRS.Query;
using CurriculumDomain.Features.GetStudentCurriculum.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Curriculums;
using CurriculumInfrastructure.Utilities;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace CurriculumInfrastructure.Features.GetStudentCurriculum.CQRS.Query;
public class GetStudentCurriculumQueryHandler : IRequestHandler<GetStudentCurriculumQuery, CommitResult<List<StudentCurriculumResponseDTO>>>
{
    private readonly HttpClient _IdentityClient;
    private readonly CurriculumDbContext _dbContext;

    public GetStudentCurriculumQueryHandler(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
        _IdentityClient = factory.CreateClient("IdentityClient");
        _IdentityClient.DefaultRequestHeaders.Add("Accept-Language", httpContextAccessor.GetAcceptLanguage());
        _IdentityClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", httpContextAccessor.GetJWTToken());
    }

    public async Task<CommitResult<List<StudentCurriculumResponseDTO>>> Handle(GetStudentCurriculumQuery request, CancellationToken cancellationToken)
    {

        // Calling Identity Micro-service to get the current grade of the user.
        CommitResult<int>? commitResult = await _IdentityClient.GetFromJsonAsync<CommitResult<int>>("/identity/GetIdentityGrade", cancellationToken);

        if (!commitResult.IsSuccess)
        {
            return commitResult.Adapt<CommitResult<List<StudentCurriculumResponseDTO>>>();
        }

        return new CommitResult<List<StudentCurriculumResponseDTO>>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Curriculum>().Where(a => a.Grade == commitResult.Value && a.IsAppShow == true).ProjectToType<StudentCurriculumResponseDTO>().ToListAsync(cancellationToken)
        };
    }
}
