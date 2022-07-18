using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;
namespace CurriculumInfrastructure.Features.Subjects.GetSubjects.CQRS.Query;

public class GetSubjectsBriefByTermQueryHandler : IRequestHandler<GetSubjectsBriefByTermQuery, CommitResults<SubjectBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public GetSubjectsBriefByTermQueryHandler(CurriculumDbContext dbContext, IDistributedCache cache)

    {
        _dbContext = dbContext;
        _cache = cache;
    }
    public async Task<CommitResults<SubjectBriefResponse>> Handle(GetSubjectsBriefByTermQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<SubjectBriefResponse>? cachedSubjectBriefResponses = await _cache.GetFromCacheAsync<string, IEnumerable<SubjectBriefResponse>>($"{request.Grade}-{request.TermId}", "Curriculum-GetSubjectsBriefByTerm", cancellationToken);

        if (cachedSubjectBriefResponses == null)
        {
            cachedSubjectBriefResponses = await _dbContext.Set<Subject>()
                                    .Where(a => a.IsAppShow == true)
                                    .Where(a => a.Grade == request.Grade && a.Term == request.TermId)
                                    .ProjectToType<SubjectBriefResponse>()
                                    .ToListAsync(cancellationToken);

            await _cache.SaveToCacheAsync($"{request.Grade}-{request.TermId}", cachedSubjectBriefResponses, "Curriculum-GetSubjectsBriefByTerm", cancellationToken);
        }

        return new CommitResults<SubjectBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedSubjectBriefResponses
        };
    }
}
