using CurriculumDomain.Features.Subjects.GetStudentSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetStudentSubjects.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.HttpClients;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using CurriculumEntities = CurriculumEntites.Entities.Subjects;
namespace CurriculumInfrastructure.Features.Subjects.GetStudentSubjects.CQRS.Query;
public class GetStudentSubjectsQueryHandler : IRequestHandler<GetStudentSubjectsQuery, CommitResults<IdnentitySubjectResponse>>
{
    private readonly IdentityClient _IdentityClient;
    private readonly CurriculumDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public GetStudentSubjectsQueryHandler(IdentityClient identityClient, CurriculumDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _IdentityClient = identityClient;
        _cache = cache;
    }

    public async Task<CommitResults<IdnentitySubjectResponse>> Handle(GetStudentSubjectsQuery request, CancellationToken cancellationToken)
    {
        //========Calling Identity Micro-service to get the current grade of the user.==============

        CommitResult<int>? commitResult = await _IdentityClient.GetStudentGradesAsync(request.StudentId, cancellationToken);


        if (!commitResult.IsSuccess)
        {
            return new CommitResults<IdnentitySubjectResponse>
            {
                ErrorCode = commitResult.ErrorCode,
                ErrorMessage = commitResult.ErrorMessage,
                ResultType = commitResult.ResultType
            };
        }

        IEnumerable<IdnentitySubjectResponse>? cachedIdnentitySubjectResponses = await _cache.GetFromCacheAsync<int, IEnumerable<IdnentitySubjectResponse>>(commitResult.Value, "Curriculum-GetStudentSubjects", cancellationToken);

        if (cachedIdnentitySubjectResponses == null)
        {
            cachedIdnentitySubjectResponses = await _dbContext.Set<CurriculumEntities.Subject>().Where(a => a.Grade == commitResult.Value && a.IsAppShow == true).ProjectToType<IdnentitySubjectResponse>().ToListAsync(cancellationToken);

            await _cache.SaveToCacheAsync(commitResult.Value, cachedIdnentitySubjectResponses, "Curriculum-GetStudentSubjects", cancellationToken);
        }
        //==================get response==================
        return new CommitResults<IdnentitySubjectResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedIdnentitySubjectResponses
        };
    }
}